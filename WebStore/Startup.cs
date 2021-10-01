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
using WebStore.Data;
using WebStore.Domain.Identity;
using WebStore.Infrastructure.Conventions;
using WebStore.Infrastructure.Middleware;
using WebStore.Services.Interfaces;
using WebStore.Services.Memory;
using WebStore.Services.SQL;

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
            services.AddDbContext<WebStoreDbContext>(opt => 
                opt.UseSqlServer(Configuration.GetConnectionString("WebStoreSql")));

            services.AddIdentity<User, Role>(/*opt => { opt. }*/)  //можно сконфигурить прямо тут
                .AddEntityFrameworkStores<WebStoreDbContext>()    //указываем, каким способом хранить - посредством EF
                .AddDefaultTokenProviders();    //это пока неясно что
            //конфигурим Identity
            services.Configure<IdentityOptions>(opt => 
            {
#if true
                //для отладочных целей ослабляем требования к паролю
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 3;
                opt.Password.RequiredUniqueChars = 1;
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

            //services.AddTransient<IEmployeeService, MemoryEmployeeService>();
            //services.AddScoped<IEmployeeService, MemoryEmployeeService>();
            services.AddSingleton<IEmployeeService, MemoryEmployeeService>();
            //services.AddScoped<IEmployeeService, DBEmployeeService>();

            //services.AddScoped<IProductService, MemoryProductService>();
            services.AddScoped<IProductService, DBProductService>();

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

                //endpoints.MapDefaultControllerRoute();
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
