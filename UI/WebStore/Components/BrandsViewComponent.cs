using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Components
{
    //[ViewComponent(Name = "BrandsVvv")]  - вместо наследования можно применить этот атрибут
    public class BrandsViewComponent : ViewComponent
    {
        private readonly IProductService _prodData;

        public BrandsViewComponent(IProductService prodData) => _prodData = prodData;

        //public async Task<IViewComponentResult> InvokeAsync() => View();  //можно и асинхронный 

        public IViewComponentResult Invoke() => View(_getBrands());

        private IEnumerable<BrandViewModel> _getBrands() => 
            _prodData.GetBrands()
                .OrderBy(b => b.Order)
                .Select(b => new BrandViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                });
    }
}
