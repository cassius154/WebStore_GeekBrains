﻿using System.Collections.Generic;
using System.Linq;
using WebStore.Services.Data;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.Services.Services.Memory
{
    public class MemoryProductService : IProductService
    {
        public IEnumerable<Brand> GetBrands() => TestData.Brands;

        public Brand GetBrandById(int id) => TestData.Brands.FirstOrDefault(b => b.Id == id);

        public IEnumerable<Section> GetSections() => TestData.Sections;

        public Section GetSectionById(int id) => TestData.Sections.FirstOrDefault(s => s.Id == id);

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

        public Product GetProductById(int id) => TestData.Products.FirstOrDefault(p => p.Id == id);
    }
}