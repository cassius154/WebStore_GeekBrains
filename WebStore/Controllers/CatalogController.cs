using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.Domain;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductService productService, ILogger<CatalogController> logger)
        {
            _productService = productService;
            _logger = logger;
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
                    .Select(p => new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                    })
            };

            return View(model);
        }
    }
}
