using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebStore.DAL.Context;

namespace WebStore.WebAPI
{
    //как переделать Startup в record
    public record Startup(IConfiguration Configuration)
    {
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
                //case "InMemory":
                //    services.AddDbContext<WebStoreDbContext>(opt =>
                //        opt.UseInMemoryDatabase("WebStore.db"));
                //    break;
            }

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
