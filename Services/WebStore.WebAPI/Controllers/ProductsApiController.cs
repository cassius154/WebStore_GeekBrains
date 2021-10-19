using Microsoft.AspNetCore.Mvc;
using WebStore.Domain;
using WebStore.Domain.DTO;
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
            return Ok(brands.ToDTO());
        }

        [HttpGet("brands/{id}")]
        public IActionResult GetBrandById(int id)
        {
            var brand = _productService.GetBrandById(id);
            return brand is null ? NotFound() : Ok(brand.ToDTO());
        }

        [HttpGet("sections")]
        public IActionResult GetSections()
        {
            var sections = _productService.GetSections();
            return Ok(sections.ToDTO());
        }

        [HttpGet("sections/{id}")]
        public IActionResult GetSectionById(int id)
        {
            var section = _productService.GetSectionById(id);
            return section is null ? NotFound() : Ok(section.ToDTO());
        }

        //тот случай, когда для получения данных используем POST - потому что передаеся сложный объект
        [HttpPost]
        public IActionResult GetProducts([FromBody] ProductFilter filter = null)
        {
            var products = _productService.GetProducts(filter);
            return Ok(products.ToDTO());
        }

        //private record ProductDTO(int Id, string Name);

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetProductById(id);
            return product is null ? NotFound() : Ok(product.ToDTO());

            //в случае использования records можно так, а не писать .ToDTO()
            //return Ok(new ProductDTO(product.Id, product.Name));
        }
    }
}
