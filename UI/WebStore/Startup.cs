using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebStore.DAL.Context;
using WebStore.Services.Data;
using WebStore.Domain.Identity;
using WebStore.Infrastructure.Conventions;
using WebStore.Infrastructure.Middleware;
using WebStore.Interfaces.Services;
using WebStore.Services.Services.Cookies;
using WebStore.Services.Services.Memory;
using WebStore.Services.Services.SQL;
using WebStore.Interfaces.TestAPI;
using WebStore.WebAPI.Clients.Values;
using WebStore.WebAPI.Clients.Employees;
using WebStore.WebAPI.Clients.Products;
using WebStore.WebAPI.Clients.Orders;
using WebStore.WebAPI.Clients.Identity;

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
            var dbType = Configuration["DatabaseType"];
            switch (dbType)
            {
                default: throw new InvalidOperationException($"Тип БД {dbType} не поддерживается.");

                case "WebStoreSql":
                    services.AddDbContext<WebStoreDbContext>(opt =>
                        opt.UseSqlServer(Configuration.GetConnectionString(dbType)));
                    break;
                case "WebStoreSqlite":
                    services.AddDbContext<WebStoreDbContext>(opt =>
                        opt.UseSqlite(Configuration.GetConnectionString(dbType),
                            //для Sqlite указываем еще библиотеку, откуда брать миграции
                            o => o.MigrationsAssembly("WebStore.DAL.Sqlite")));
                    break;
                case "InMemory":
                    services.AddDbContext<WebStoreDbContext>(opt =>
                        opt.UseInMemoryDatabase("WebStore.db"));
                    break;
            }

            services.AddIdentity<User, Role>(/*opt => { opt. }*/)  //можно сконфигурить прямо тут
                //.AddEntityFrameworkStores<WebStoreDbContext>()    //указываем, каким способом хранить - посредством EF
                .AddDefaultTokenProviders();    //это пока неясно что


            //можем разделить клиентов основных данных и Identity по разным адресам
            //services.AddIdentityWebStoreWebAPIClients();
            services.AddHttpClient("WebStoreWebAPIIdentity", client => client.BaseAddress = new(Configuration["WebAPI"]))
               .AddTypedClient<IUserStore<User>, UsersClient>()
               .AddTypedClient<IUserRoleStore<User>, UsersClient>()
               .AddTypedClient<IUserPasswordStore<User>, UsersClient>()
               .AddTypedClient<IUserEmailStore<User>, UsersClient>()
               .AddTypedClient<IUserPhoneNumberStore<User>, UsersClient>()
               .AddTypedClient<IUserTwoFactorStore<User>, UsersClient>()
               .AddTypedClient<IUserClaimStore<User>, UsersClient>()
               .AddTypedClient<IUserLoginStore<User>, UsersClient>()
               .AddTypedClient<IRoleStore<Role>, RolesClient>()
                ;

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

            services.AddTransient<WebStoreDbInitializer>();

            ////services.AddTransient<IEmployeeService, MemoryEmployeeService>();
            ////services.AddScoped<IEmployeeService, MemoryEmployeeService>();
            //services.AddSingleton<IEmployeeService, MemoryEmployeeService>();
            ////services.AddScoped<IEmployeeService, DBEmployeeService>();

            //services.AddScoped<IProductService, MemoryProductService>();
            //services.AddScoped<IProductService, DBProductService>();
            //services.AddScoped<IOrderService, DBOrderService>();
            services.AddScoped<ICartService, CookiesCartService>();

            //services.AddHttpClient<IValuesClient, ValuesClient>(client => client.BaseAddress = new Uri(Configuration["WebAPI"]));
            services.AddHttpClient("WebStoreWebAPI", client => client.BaseAddress = new(Configuration["WebAPI"]))
               .AddTypedClient<IValuesClient, ValuesClient>()
               .AddTypedClient<IEmployeeService, EmployeesClient>()
               .AddTypedClient<IProductService, ProductsClient>()
               .AddTypedClient<IOrderService, OrderClient>();

              services.AddControllersWithViews(opt => opt.Conventions.Add(new TestControllerConvention()))
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
