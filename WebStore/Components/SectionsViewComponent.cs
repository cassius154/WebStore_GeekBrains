using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Components
{
    public class SectionsViewComponent : ViewComponent
    {
        private readonly IProductData _prodData;

        public SectionsViewComponent(IProductData prodData)
        {
            _prodData = prodData;
        }

        public IViewComponentResult Invoke()
        {
            var sections = _prodData.GetSections();
            var parents = sections.Where(s => s.ParentId is null);

            var parentViews = parents
                .Select(p => new SectionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Order = p.Order,
                })
                .ToList();

            foreach(var pv in parentViews)
            {
                var childs = sections.Where(s => s.ParentId == pv.Id);

                foreach (var child in childs)
                {
                    pv.Childs.Add(new SectionViewModel
                    {
                        Id = child.Id,
                        Name = child.Name,
                        Order = child.Order,
                        Parent = pv,
                    });
                }

                pv.Childs.Sort((a, b) => Comparer<int>.Default.Compare(a.Order, b.Order));
            }

            parentViews.Sort((a, b) => Comparer<int>.Default.Compare(a.Order, b.Order));

            return View(parentViews);
        }

        //public async Task<IViewComponentResult> InvokeAsync() => View();  //можно и асинхронный 
    }
}
