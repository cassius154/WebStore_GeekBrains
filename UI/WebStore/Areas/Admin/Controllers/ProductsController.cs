using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Identity;
using WebStore.Interfaces.Services;

namespace WebStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Administrators)]

    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService) => _productService = productService;

        public IActionResult Index()
        {
            var products = _productService.GetProducts();
            return View(products.Products);
        }

        public IActionResult Edit(int id) => RedirectToAction(nameof(Index));

        public IActionResult Delete(int id) => RedirectToAction(nameof(Index));
    }
}
