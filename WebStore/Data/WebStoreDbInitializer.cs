﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.Context;

namespace WebStore.Data
{
    public class WebStoreDbInitializer
    {
        private readonly WebStoreDbContext _db;
        private readonly ILogger<WebStoreDbInitializer> _logger;

        public WebStoreDbInitializer(WebStoreDbContext db, ILogger<WebStoreDbInitializer> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Запуск инициализации БД");

            //var deleted = await _db.Database.EnsureDeletedAsync();  //если надо удалить БД при старте
            //var created = await _db.Database.EnsureCreatedAsync(); //целесообразно вызывать, если нет миграций

            //если есть миграции, то...
            //список ожидающих миграций
            var pendingMigrations = await _db.Database.GetPendingMigrationsAsync();
            //список уже примененных миграций
            var appliedMigrations = await _db.Database.GetAppliedMigrationsAsync();

            if (pendingMigrations.Any())
            {
                _logger.LogInformation($"Применение миграций: {string.Join(",", pendingMigrations)}");
                await _db.Database.MigrateAsync();
            }

            await _initProductsAsync();
        }

        private async Task _initProductsAsync()
        {
            if (_db.Products.Any())
            {
                _logger.LogInformation("Инициализация БД нформацией о товарах не требуется.");
                return;
            }

            //await using (var tx = await _db.Database.BeginTransactionAsync())
            await using (await _db.Database.BeginTransactionAsync())
            {
                _logger.LogInformation("Добавление секций...");
                await _db.Sections.AddRangeAsync(TestData.Sections);
                //отключаем генерацию ID на сервере - вставляем свои из TestData
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Sections] ON");
                await _db.SaveChangesAsync();
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Sections] OFF");
                _logger.LogInformation("Добавление секций выполнено успешно");

                _logger.LogInformation("Добавление брендов...");
                await _db.Brands.AddRangeAsync(TestData.Brands);
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Brands] ON");
                await _db.SaveChangesAsync();
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Brands] OFF");
                _logger.LogInformation("Добавление брендов выполнено успешно");

                _logger.LogInformation("Добавление товаров...");
                await _db.Products.AddRangeAsync(TestData.Products);
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Products] ON");
                await _db.SaveChangesAsync();
                await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Products] OFF");
                _logger.LogInformation("Добавление товаров выполнено успешно");

                //await tx.CommitAsync();
                await _db.Database.CommitTransactionAsync();
            }
        }
    }
}
