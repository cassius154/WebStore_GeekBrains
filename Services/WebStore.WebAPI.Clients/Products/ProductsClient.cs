using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using WebStore.Domain;
using WebStore.Domain.DTO;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Products
{
    public class ProductsClient : ClientBase, IProductService
    {
        public ProductsClient(HttpClient client) : base(client, "api/products") { }

        public Brand GetBrandById(int id)
        {
            var brand = Get<BrandDTO>($"{address}/brands/{id}");
            return brand.FromDTO();
        }

        public IEnumerable<Brand> GetBrands()
        {
            var brands = Get<IEnumerable<BrandDTO>>($"{address}/brands");
            return brands.FromDTO();
        }

        public Product GetProductById(int id)
        {
            var product = Get<ProductDTO>($"{address}/{id}");
            return product.FromDTO();
        }

        public IEnumerable<Product> GetProducts(ProductFilter filter = null)
        {
            var response = Post(address, filter ?? new());
            var products_dtos = response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>().Result;
            return products_dtos.FromDTO();
        }

        public Section GetSectionById(int id)
{
            var section = Get<SectionDTO>($"{address}/sections/{id}");
            return section.FromDTO();
        }

        public IEnumerable<Section> GetSections()
        {
            var sections = Get<IEnumerable<SectionDTO>>($"{address}/sections");
            return sections.FromDTO();
        }
    }
}
