using System.Diagnostics;
using System.Linq;
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

            var timer = Stopwatch.StartNew();

            //связываем данные логически - т.е. вместо ссылок вида xxxId прописываем прямиком сущности
            var dictSections = TestData.Sections.ToDictionary(s => s.Id);
            var dictBrands = TestData.Brands.ToDictionary(b => b.Id);

            //проставляем секциям целиком parent-ы вместо ParentId 
            foreach(var childSection in TestData.Sections.Where(s => s.ParentId is not null))
            {
                childSection.Parent = dictSections[(int)childSection.ParentId!];  //! - это чтобы при какой-то включенной опции не выдавало предупреждений о непроверке на null
            }

            foreach (var product in TestData.Products)
            {
                product.Section = dictSections[product.SectionId];
                //if (product.BrandId is { } brandId)  //новая форма - к ней привыкнуть надо. Из плюсов - не нужно .Value или приведение к (int) - тут сразу дается полноценный int
                //{
                //    product.Brand = dictBrands[brandId];
                //}
                if (product.BrandId is not null)
                {
                    product.Brand = dictBrands[product.BrandId.Value];
                }

                //чистим ключи - БД сама расставит их как надо, из атрибута [DatabaseGenerated(DatabaseGeneratedOption.Identity)] и внешние из логических связей
                product.Id = 0;
                product.SectionId = 0;
                product.BrandId = null;
            }

            //чистим ключи у секций
            foreach (var section in TestData.Sections)
            {
                section.Id = 0;  //по-моему это лишнее - тут сработает [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
                section.ParentId = null;
            }

            //чистим ключи у брендов
            foreach (var brand in TestData.Brands)
            {
                brand.Id = 0;  //по-моему это лишнее - тут сработает [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            }

            _logger.LogInformation("Добавление каталога...");
            //await using (var tx = await _db.Database.BeginTransactionAsync())
            await using (await _db.Database.BeginTransactionAsync())
            {
                await _db.Brands.AddRangeAsync(TestData.Brands);
                await _db.Sections.AddRangeAsync(TestData.Sections);
                await _db.Products.AddRangeAsync(TestData.Products);

                await _db.SaveChangesAsync();
                //await tx.CommitAsync();
                await _db.Database.CommitTransactionAsync();
            }
            _logger.LogInformation($"Добавление каталога выполнено успешно за {timer/*ElapsedMilliseconds*/.Elapsed.TotalMilliseconds} мс");
        }
    }
}
