using System;
using System.Data;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.Context;
using WebStore.Domain.Identity;

namespace WebStore.Data
{
    public class WebStoreDbInitializer
    {
        private readonly WebStoreDbContext _db;
        private readonly ILogger<WebStoreDbInitializer> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public WebStoreDbInitializer(WebStoreDbContext db,
            ILogger<WebStoreDbInitializer> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _db = db;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
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

            try
            {
                await _initProductsAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Ошибка инициализации каталога товаров");
                throw;
            }

            try
            {
                //await _initAdministratorsAsync();
                await _initIdentityAsync();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Ошибка инициализации системы Identity");
                throw;
            }
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
            foreach (var childSection in TestData.Sections.Where(s => s.ParentId is not null))
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

        private async Task _initIdentityAsync()
        {
            _logger.LogInformation("Инициализация системы Identity");
            var timer = Stopwatch.StartNew();
            
            async Task checkRoleAsync(string roleName)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    _logger.LogInformation($"Роль {roleName} существует.");
                }
                else
                {
                    _logger.LogInformation($"Роль {roleName} не существует.");
                    var identityResult = await _roleManager.CreateAsync(new Role { Name = roleName });
                    if (!identityResult.Succeeded)
                    {
                        throw new Exception($"Ошибки создания роли: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    }
                    _logger.LogInformation($"Роль {roleName} успешно создана.");
                }
            }

            await checkRoleAsync(Role.Administrators);
            await checkRoleAsync(Role.Users);

            if (await _userManager.FindByNameAsync(User.AdmLogin) is null)
            {
                _logger.LogInformation($"Пользователь {User.AdmLogin} не существует.");

                var admin = new User
                {
                    UserName = User.AdmLogin,
                };

                var identityResult = await _userManager.CreateAsync(admin, User.DefaultAdmPassword);
                if (identityResult.Succeeded)
                {
                    _logger.LogInformation($"Пользователь {admin.UserName} успешно создан.");
                    identityResult = await _userManager.AddToRoleAsync(admin, Role.Administrators);
                    _logger.LogInformation($"Пользователю {admin.UserName} успешно добавлена роль {Role.Administrators}.");
                }
                else
                {
                    var errors = identityResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Учетная запись администратора не создана! Ошибки: {string.Join(", ", errors)}");

                    throw new InvalidOperationException($"Невозможно создать администратора {string.Join(", ", errors)}");
                }
            }
            _logger.LogInformation($"Данные системы Identity успешно добавлены в БД за {timer.Elapsed.TotalMilliseconds}мс");
        }

        private async Task _initAdministratorsAsync()
        {
            IdentityResult identityResult;
            User user = null;

            var ok = await _roleManager.RoleExistsAsync(Role.Administrators);

            if (!ok)
            {
                var role = new Role
                {
                    Name = Role.Administrators,
                };

                _logger.LogInformation("Создание роли администратора...");
                identityResult = await _roleManager.CreateAsync(role);

                ok = _handleResult(identityResult,
                    "Создание роли администратора выполнено успешно.",
                    "Ошибка создания роли администратора: {0}"
                );
            }

            if (ok)
            {
                user = await _userManager.FindByNameAsync(User.AdmLogin);
                if (user is null)
                {
                    user = new User
                    {
                        UserName = User.AdmLogin,
                    };

                    _logger.LogInformation("Создание администратора...");
                    identityResult = await _userManager.CreateAsync(user, User.DefaultAdmPassword);
                    ok = _handleResult(identityResult,
                        "Создание администратора выполнено успешно.",
                        "Ошибка создания администратора: {0}"
                    );

                    if (ok)
                    {
                        user = await _userManager.FindByNameAsync(User.AdmLogin);
                    }
                }
            }

            if (ok)
            {
                if (user != null && !(await _userManager.IsInRoleAsync(user, Role.Administrators)))
                {
                    _logger.LogInformation("Добавление администратора в группу администраторов...");
                    identityResult = await _userManager.AddToRoleAsync(user, Role.Administrators);
                    ok = _handleResult(identityResult,
                        "Добавление администратора в группу администраторов выполнено успешно.",
                        "Ошибка добавления администратора в группу администраторов: {0}"
                    );
                }
            }
        }

        private bool _handleResult(IdentityResult result, string okMessage, string errMessage)
        {
            var ret = result.Succeeded;
            _logger.LogInformation(ret ? okMessage : string.Format(errMessage, string.Join(", ", result.Errors.Select(e => e.Description))));

            return ret;
        }

    }
}
