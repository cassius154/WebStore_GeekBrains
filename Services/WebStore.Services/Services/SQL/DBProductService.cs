using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebStore.Services.Services.SQL
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
            //без AsNoTracking возвращаются элементы с установленными Parent
            //с установленным AsNoTracking все Parent = null
            return _db.Sections; //.AsNoTracking(); //.ToList();
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

        public ProductsPage GetProducts(ProductFilter filter = null)
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

            var totalCount = ret.Count();
            //if (filter?.PageSize > 0 && filter?.Page > 0)
            if (filter is { PageSize: > 0 and var pageSize, Page: > 0 and var pageNumber })
            {
                ret = ret
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize);
            }

            //return ret.AsNoTracking(); //.ToList();
            //return new(ret.AsNoTracking().AsEnumerable(), totalCount);
            return new(ret.AsNoTracking(), totalCount);
        }


    }
}
