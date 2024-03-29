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
using WebStore.Hubs;

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
            services.AddIdentity<User, Role>(/*opt => { opt. }*/)  //����� ������������ ����� ���
                .AddIdentityWebStoreWebAPIClients()
                .AddDefaultTokenProviders();    //��� ���� ������ ���


            //����� ��������� �������� �������� ������ � Identity �� ������ �������
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

            //���������� Identity
            services.Configure<IdentityOptions>(opt => 
            {
#if true  //��������, DEBUG
                //��� ���������� ����� ��������� ���������� � ������
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

            //����� ���� ����������� ������� Identity ��������� ���� ����� � �����
            //���������� �����
            services.ConfigureApplicationCookie(opt => 
            {
                opt.Cookie.Name = "GB.WebStore";
                opt.Cookie.HttpOnly = true;  //�������� ������������, �������� ��� ������ ������ �������� �������, ����� http

                opt.ExpireTimeSpan = TimeSpan.FromDays(10);

                opt.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login"); //"/Account/Login";
                opt.LogoutPath = "/Account/Logout";
                opt.AccessDeniedPath = "/Account/AccessDenied";

                //�������������� ������ Identity ��� ����� ����������� ������������� (���������), ������� ���������� � ������, � ��������� ��� � ������
                //����� ������ ���� ��������� id ���� ������� �� id ��������� �����, ����� ���������� ����� ��� ����� ������
                //���� �������� � �������� �� �������������� ����� ���������� id �� ���������� ��� ������
                opt.SlidingExpiration = true;
            });


            //������� ������������ ������� ������� ��� ������
            //services.AddScoped<ICartService, CookiesCartService>();
            services.AddScoped<ICartStore, CookiesCartStore>();
            services.AddScoped<ICartService, CartService>();

            //services.AddHttpClient<IValuesClient, ValuesClient>(client => client.BaseAddress = new Uri(Configuration["WebAPI"]));
            services.AddHttpClient("WebStoreWebAPI", client => client.BaseAddress = new(Configuration["WebAPI"]))
               .AddTypedClient<IValuesClient, ValuesClient>()
               .AddTypedClient<IEmployeeService, EmployeesClient>()
               .AddTypedClient<IProductService, ProductsClient>()
               .AddTypedClient<IOrderService, OrderClient>()
               .SetHandlerLifetime(TimeSpan.FromMinutes(5))     // ������� ��� HttpClient �������� � �������� ��� �� ������� (5 �����)
               .AddPolicyHandler(getRetryPolicy())              // �������� ��������� �������� � ������ ���� WebAPI �� ��������
               .AddPolicyHandler(getCircuitBreakerPolicy());    // ���������� ������������� ����������� �������� � ������� ������������� �������

            static IAsyncPolicy<HttpResponseMessage> getRetryPolicy(int maxRetryCount = 5, int maxJitterTime = 1000)
            {
                var jitter = new Random();
                return HttpPolicyExtensions
                   //�� ������������ ����� ����������� http-������ (404, 403 � �.�.)
                   .HandleTransientHttpError()                          
                   .WaitAndRetryAsync(maxRetryCount, RetryAttempt =>
                        //���-�� �������� � �������� �������� -
                        //��������������� ��������� �� ������� ������
                        //(1 ���, 2 ���, 4, 8, 16, 32)
                        TimeSpan.FromSeconds(Math.Pow(2, RetryAttempt)) +
                        //����� �� ������� � ��������� "��������",
                        //����� ������� ����� ���� ������� �������, � �������������������
                        //��������� ��������� ���-�� �����������
                        TimeSpan.FromMilliseconds(jitter.Next(0, maxJitterTime)));
            }

            static IAsyncPolicy<HttpResponseMessage> getCircuitBreakerPolicy() =>
                HttpPolicyExtensions
                   .HandleTransientHttpError()
                   //��� � ������� ������� ������������ ���������� id
                   //� �����������, ��� �� ���� id �� ���������� �����
                   .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, TimeSpan.FromSeconds(30));

            services.AddControllersWithViews(opt => opt.Conventions.Add(new TestControllerConvention()))
               .AddRazorRuntimeCompilation();

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory log)
        {
            log.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            //������ ������ �� ��������� ����� ������ ���� ����� ��������, ����� ������� �� ��� �������
            //���� � ���������� UseStatusCodePages
            //app.UseStatusCodePages();
            app.UseStatusCodePagesWithRedirects("~/Home/Status/{0}");

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();  //���� ����� ������ � blazor-������ index.html

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
                endpoints.MapHub<ChatHub>("/chat");

                endpoints.MapGet("/greetings", async context =>
                {
                    await context.Response.WriteAsync(Configuration["Greeting"]);
                });

                //��� ��, ��� ������������ ��� ���������� Area - ��������� �� ������������ �����
                endpoints.MapControllerRoute(
                            name: "areas",
                            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                          );

                //endpoints.MapDefaultControllerRoute();
                endpoints.MapControllerRoute(
                    "default",
                    //��-�� MapFallbackToFile ������� ��� Home
                    //���� ���������� ������ - �� ��������� �������� Index
                    //����� - MapFallbackToFile �������� �� index.html
                    "{controller}/{action=Index}/{id?}");
                    //"{controller=Home}/{action=Index}/{id?}");

                //��� ������������� �������� ����� ������������ �� index.html (� ��� Blazor ����� :) )
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
