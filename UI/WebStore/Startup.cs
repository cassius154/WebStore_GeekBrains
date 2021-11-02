using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly.Extensions.Http;
using Polly;
using WebStore.Domain.Identity;
using WebStore.Infrastructure.Conventions;
using WebStore.Infrastructure.Middleware;
using WebStore.Interfaces.Services;
using WebStore.Interfaces.TestAPI;
using WebStore.Logger;
using WebStore.Services.Services;
using WebStore.Services.Services.Cookies;
using WebStore.WebAPI.Clients.Employees;
using WebStore.WebAPI.Clients.Identity;
using WebStore.WebAPI.Clients.Orders;
using WebStore.WebAPI.Clients.Products;
using WebStore.WebAPI.Clients.Values;

namespace WebStore
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<User, Role>(/*opt => { opt. }*/)  //можно сконфигурить прямо тут
                .AddIdentityWebStoreWebAPIClients()
                .AddDefaultTokenProviders();    //это пока неясно что


            //можем разделить клиентов основных данных и Identity по разным адресам
            //services.AddIdentityWebStoreWebAPIClients();
            //services.AddHttpClient("WebStoreWebAPIIdentity", client => client.BaseAddress = new(Configuration["WebAPI"]))
            //   .AddTypedClient<IUserStore<User>, UsersClient>()
            //   .AddTypedClient<IUserRoleStore<User>, UsersClient>()
            //   .AddTypedClient<IUserPasswordStore<User>, UsersClient>()
            //   .AddTypedClient<IUserEmailStore<User>, UsersClient>()
            //   .AddTypedClient<IUserPhoneNumberStore<User>, UsersClient>()
            //   .AddTypedClient<IUserTwoFactorStore<User>, UsersClient>()
            //   .AddTypedClient<IUserClaimStore<User>, UsersClient>()
            //   .AddTypedClient<IUserLoginStore<User>, UsersClient>()
            //   .AddTypedClient<IRoleStore<Role>, RolesClient>()
            //    ;

            //конфигурим Identity
            services.Configure<IdentityOptions>(opt => 
            {
#if true  //например, DEBUG
                //для отладочных целей ослабляем требования к паролю
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 3;
                opt.Password.RequiredUniqueChars = 3;
#endif
                opt.User.RequireUniqueEmail = false;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                opt.Lockout.AllowedForNewUsers = false;
                opt.Lockout.MaxFailedAccessAttempts = 10;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            });

            //когда юзер залогинился система Identity сохраняет факт входа в кукис
            //конфигурим кукис
            services.ConfigureApplicationCookie(opt => 
            {
                opt.Cookie.Name = "GB.WebStore";
                opt.Cookie.HttpOnly = true;  //повышает безопасность, отключая все другие каналы передачи кукисов, кроме http

                opt.ExpireTimeSpan = TimeSpan.FromDays(10);

                opt.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login"); //"/Account/Login";
                opt.LogoutPath = "/Account/Logout";
                opt.AccessDeniedPath = "/Account/AccessDenied";

                //незалогиненным юзерам Identity все равно присваивает идентификатор (анонимуса), который существует в сеансе, и сохраняет его в кукисе
                //после логина этот анонимный id надо сменить на id вошедшего юзера, иначе существуют атаки для увода данных
                //этот параметр и отвечает за принудительную смену анонимного id на правильный при логине
                opt.SlidingExpiration = true;
            });


            //провели декомпозицию сервиса корзины для тестов
            //services.AddScoped<ICartService, CookiesCartService>();
            services.AddScoped<ICartStore, CookiesCartStore>();
            services.AddScoped<ICartService, CartService>();

            //services.AddHttpClient<IValuesClient, ValuesClient>(client => client.BaseAddress = new Uri(Configuration["WebAPI"]));
            services.AddHttpClient("WebStoreWebAPI", client => client.BaseAddress = new(Configuration["WebAPI"]))
               .AddTypedClient<IValuesClient, ValuesClient>()
               .AddTypedClient<IEmployeeService, EmployeesClient>()
               .AddTypedClient<IProductService, ProductsClient>()
               .AddTypedClient<IOrderService, OrderClient>()
               .SetHandlerLifetime(TimeSpan.FromMinutes(5))     // Создать кеш HttpClient объектов с очисткой его по времени (5 минут)
               .AddPolicyHandler(getRetryPolicy())              // Политика повторных запросов в случае если WebAPI не отвечает
               .AddPolicyHandler(getCircuitBreakerPolicy());    // Разрушение потенциальных циклических запросов в большой распределённой системе

            static IAsyncPolicy<HttpResponseMessage> getRetryPolicy(int maxRetryCount = 5, int maxJitterTime = 1000)
            {
                var jitter = new Random();
                return HttpPolicyExtensions
                   //не обрабатывать здесь стандартные http-ошибки (404, 403 и т.д.)
                   .HandleTransientHttpError()                          
                   .WaitAndRetryAsync(maxRetryCount, RetryAttempt =>
                        //кол-во повторов и политика повторов -
                        //увеличивающиеся интервалы на степень двойки
                        //(1 сек, 2 сек, 4, 8, 16, 32)
                        TimeSpan.FromSeconds(Math.Pow(2, RetryAttempt)) +
                        //чтобы не входить в ритмичный "резонанс",
                        //когда запросы будут идти четкими ритмами, и рассинхронизировать
                        //добавляем случайное кол-во миллисекунд
                        TimeSpan.FromMilliseconds(jitter.Next(0, maxJitterTime)));
            }

            static IAsyncPolicy<HttpResponseMessage> getCircuitBreakerPolicy() =>
                HttpPolicyExtensions
                   .HandleTransientHttpError()
                   //тут к каждому запросу дописывается уникальный id
                   //и проверяется, что по этим id не образуются циклы
                   .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, TimeSpan.FromSeconds(30));

            services.AddControllersWithViews(opt => opt.Conventions.Add(new TestControllerConvention()))
               .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory log)
        {
            log.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            //внутри ответа со статусным кодом должен быть адрес страницы, чтобы браузер на нее перешел
            //этим и занимается UseStatusCodePages
            //app.UseStatusCodePages();
            app.UseStatusCodePagesWithRedirects("~/Home/Status/{0}");

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<TestMiddleware>();

            //app.UseWelcomePage();
            //app.UseWelcomePage("/welcome");

            //app.UseStatusCodePagesWithReExecute("/Home/Status/{0}");

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/greetings", async context =>
                {
                    await context.Response.WriteAsync(Configuration["Greeting"]);
                });

                //это то, что нагенерилось при добавлении Area - перенесли из сгенеренного файла
                endpoints.MapControllerRoute(
                            name: "areas",
                            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                          );

                //endpoints.MapDefaultControllerRoute();
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
