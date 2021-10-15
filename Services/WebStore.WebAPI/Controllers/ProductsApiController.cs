using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Entities;
using WebStore.Domain;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers
{

    [ApiController]
    [Route("api/products")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsApiController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("brands")]
        public IActionResult GetBrands()
        {
            var brands = _productService.GetBrands();
            return Ok(brands);
        }

        [HttpGet("brands/{id}")]
        public IActionResult GetBrandById(int id)
        {
            var brand = _productService.GetBrandById(id);
            return brand is null ? NotFound() : Ok(brand);
        }

        [HttpGet("sections")]
        public IActionResult GetSections()
        {
            var sections = _productService.GetSections();
            return Ok(sections);
        }

        [HttpGet("sections/{id}")]
        public IActionResult GetSectionById(int id)
        {
            var section = _productService.GetSectionById(id);
            return section is null ? NotFound() : Ok(section);
        }

        //тот случай, когда для получения данных используем POST - потому что передаеся сложный объект
        [HttpPost]
        public IActionResult GetProducts([FromBody] ProductFilter filter = null)
        {
            var products = _productService.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetProductById(id);
            return product is null ? NotFound() : Ok(product);
        }
    }
}
