using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Services.Interfaces;

namespace WebStore.Services.SQL
{
    public class DBProductService : IProductService
    {
        private readonly WebStoreDbContext _db;

        public DBProductService(WebStoreDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Brand> GetBrands()
        {
            return _db.Brands.AsNoTracking(); //.ToList();  //должно сработать и без ToList() - сработает при первом foreach где-нибудь во View
        }

        public Brand GetBrandById(int id) => _db.Brands.Find(id);

        public IEnumerable<Section> GetSections()
        {
            return _db.Sections.AsNoTracking(); //.ToList();
        }

        public Section GetSectionById(int id) => _db.Sections.SingleOrDefault(s => s.Id == id);

        public Product GetProductById(int id)
        {
            return _db.Products
                .Include(p => p.Section)
                .Include(p => p.Brand)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Product> GetProducts(ProductFilter filter = null)
        {
            IQueryable<Product> ret = _db.Products
                .Include(p => p.Section)
                .Include(p => p.Brand);

            if (filter?.Ids?.Length > 0)
            {
                ret = ret.Where(p => filter.Ids.Contains(p.Id));
            }
            else
            {
                if (filter?.BrandId is { } brandId)
                {
                    ret = ret.Where(p => p.BrandId == brandId);
                }

                if (filter?.SectionId is not null)
                {
                    ret = ret.Where(p => p.SectionId == filter.SectionId);
                }
            }

            return ret.AsNoTracking(); //.ToList();
        }


    }
}
