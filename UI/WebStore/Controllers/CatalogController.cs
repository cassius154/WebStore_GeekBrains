using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.Domain;
using WebStore.Domain.ViewModels;
using WebStore.Services.DTO;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductService productService) //, ILogger<CatalogController> logger)
        {
            _productService = productService;
            //_logger = logger;
        }

        public IActionResult Index(int? brandId, int? sectionId)
        {
            var filter = new ProductFilter
            {
                BrandId = brandId,
                SectionId = sectionId,
            };
            var products = _productService.GetProducts(filter);

            var model = new CatalogViewModel
            {
                BrandId = brandId,
                SectionId = sectionId,
                Products = products
                    .OrderBy(p => p.Order)
                    .ToProductView()
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
