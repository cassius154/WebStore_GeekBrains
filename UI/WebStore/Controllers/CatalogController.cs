using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.Domain;
using WebStore.Domain.ViewModels;
using WebStore.Services.DTO;
using WebStore.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebStore.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductService _productService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductService productService,
            IConfiguration configuration,
            ILogger<CatalogController> logger)
        {
            _productService = productService;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index(int? brandId, int? sectionId, int page = 1, int? pageSize = null)
        {
            var pSize = pageSize
                ?? (int.TryParse(_configuration["CatalogPageSize"], out var value) ? value : null);

            var filter = new ProductFilter
            {
                BrandId = brandId,
                SectionId = sectionId,
                Page = page,
                PageSize = pSize,
            };

            var (products, totalCount) = _productService.GetProducts(filter);

            var model = new CatalogViewModel
            {
                BrandId = brandId,
                SectionId = sectionId,
                
                Products = products
                    .OrderBy(p => p.Order)
                    .ToProductView(),
                
                PageViewModel = new()
                {
                    Page = page,
                    PageSize = pSize ?? 0,
                    TotalItems = totalCount,
                }
            };

            return View(model);
        }

        public IActionResult Details(int Id)
        {
            var product = _productService.GetProductById(Id);

            if (product is null)
            {
                return NotFound();
            }

            return View(product.ToProductView());
        }
    }
}
