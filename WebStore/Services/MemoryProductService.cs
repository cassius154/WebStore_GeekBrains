using System.Collections.Generic;
using System.Linq;
using WebStore.Data;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class MemoryProductService : IProductService
    {
        public IEnumerable<Brand> GetBrands() => TestData.Brands;

        public IEnumerable<Section> GetSections() => TestData.Sections;

        public IEnumerable<Product> GetProducts(ProductFilter filter = null)
        {
            var ret = TestData.Products;
            //if (filter?.BrandId is not null)
            //{
            //    ret = ret.Where(p => p.BrandId == filter.BrandId);
            //}
            //или новая форма
            if (filter?.BrandId is { } brandId)
            {
                ret = ret.Where(p => p.BrandId == brandId);
            }

            if (filter?.SectionId is not null)
            {
                ret = ret.Where(p => p.SectionId == filter.SectionId);
            }

            return ret;
        }
    }
}