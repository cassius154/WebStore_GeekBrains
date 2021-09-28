using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;

namespace WebStore.Data
{
    public class WebStoreDbInitializer
    {
        private readonly WebStoreDbContext _db;

        public WebStoreDbInitializer(WebStoreDbContext db) => _db = db;

        public async Task InitializeAsync()
        {
            //var deleted = await _db.Database.EnsureDeletedAsync();  //если надо удалить БД при старте
            //var created = await _db.Database.EnsureCreatedAsync(); //целесообразно вызывать, если нет миграций

            //если есть миграции, то...
            //список ожидающих миграций
            var pendingMigrations = await _db.Database.GetPendingMigrationsAsync();
            //список уже примененных миграций
            var appliedMigrations = await _db.Database.GetAppliedMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await _db.Database.MigrateAsync();
            }

            await _initProductsAsync();
        }

        private async Task _initProductsAsync()
        {
            //await using (var tx = await _db.Database.BeginTransactionAsync())
            await using (await _db.Database.BeginTransactionAsync())
            {
                await _db.Sections.AddRangeAsync(TestData.Sections);
                //отключаем генерацию ID на сервере - вставляем свои из TestData
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Sections] ON");
                await _db.SaveChangesAsync();
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Sections] OFF");

                await _db.Brands.AddRangeAsync(TestData.Brands);
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Brands] ON");
                await _db.SaveChangesAsync();
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Brands] OFF");

                await _db.Products.AddRangeAsync(TestData.Products);
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Products] ON");
                await _db.SaveChangesAsync();
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Products] OFF");

                //await tx.CommitAsync();
                await _db.Database.CommitTransactionAsync();
            }
        }
    }
}
