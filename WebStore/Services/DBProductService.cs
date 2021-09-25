using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebStore.Data;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class DBProductService : IProductService
    {
        private WebStoreDbContext _db;

        public DBProductService(WebStoreDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Brand> GetBrands()
        {
            return _db.Brands.ToList();
        }

        public IEnumerable<Product> GetProducts(ProductFilter filter = null)
        {
            IQueryable<Product> ret = _db.Products;
            if (filter?.BrandId is { } brandId)
            {
                ret = ret.Where(p => p.BrandId == brandId);
            }

            if (filter?.SectionId is not null)
            {
                ret = ret.Where(p => p.SectionId == filter.SectionId);
            }

            return ret.ToList();
        }

        public IEnumerable<Section> GetSections()
        {
            return _db.Sections.ToList();
        }
    }
}
