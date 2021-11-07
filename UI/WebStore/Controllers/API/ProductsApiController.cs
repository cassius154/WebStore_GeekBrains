﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers.API
{
    [ApiController, Route("api/products")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsApiController(IProductService productData) => _productService = productData;

        private class ProductInfo
        {
            public int Id { get; set; }

            public string Title { get; set; }

            public decimal Price { get; set; }

            public string Image { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(3500);

            var products = _productService.GetProducts();

            var infos = products.Products.Select(p => new ProductInfo
            {
                Id = p.Id,
                Title = p.Name,
                Price = p.Price,
                Image = p.ImageUrl
            });

            return Ok(infos);
        }
    }
}