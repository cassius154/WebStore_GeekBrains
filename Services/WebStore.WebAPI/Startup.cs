using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebStore.DAL.Context;
using WebStore.Domain.Identity;
using WebStore.Interfaces.Services;
using WebStore.Services.Data;
using WebStore.Services.Services.Memory;
using WebStore.Services.Services.SQL;

namespace WebStore.WebAPI
{
    //��� ���������� Startup � record
    public record Startup(IConfiguration Configuration)
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var dbType = Configuration["DatabaseType"];
            switch (dbType)
            {
                default: throw new InvalidOperationException($"��� �� {dbType} �� ��������������.");

                case "WebStoreSql":
                    services.AddDbContext<WebStoreDbContext>(opt =>
                        opt.UseSqlServer(Configuration.GetConnectionString(dbType)));
                    break;
                case "WebStoreSqlite":
                    services.AddDbContext<WebStoreDbContext>(opt =>
                        opt.UseSqlite(Configuration.GetConnectionString(dbType),
                            //��� Sqlite ��������� ��� ����������, ������ ����� ��������
                            o => o.MigrationsAssembly("WebStore.DAL.Sqlite")));
                    break;
                //case "InMemory":
                //    services.AddDbContext<WebStoreDbContext>(opt =>
                //        opt.UseInMemoryDatabase("WebStore.db"));
                //    break;
            }

            services.AddScoped<WebStoreDbInitializer>();

            services.AddIdentity<User, Role>(/*opt => { opt. }*/)  //����� ������������ ����� ���
                .AddEntityFrameworkStores<WebStoreDbContext>()    //���������, ����� �������� ������� - ����������� EF
                .AddDefaultTokenProviders();    //��� ���� ������ ���
            //���������� Identity
            services.Configure<IdentityOptions>(opt =>
            {
#if true
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


            //services.AddTransient<IEmployeeService, MemoryEmployeeService>();
            //services.AddScoped<IEmployeeService, MemoryEmployeeService>();
            services.AddSingleton<IEmployeeService, MemoryEmployeeService>();
            //services.AddScoped<IEmployeeService, DBEmployeeService>();

            //services.AddScoped<IProductService, MemoryProductService>();
            services.AddScoped<IProductService, DBProductService>();
            services.AddScoped<IOrderService, DBOrderService>();
            //services.AddScoped<ICartService, CookiesCartService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebStore.WebAPI", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebStore.WebAPI v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
