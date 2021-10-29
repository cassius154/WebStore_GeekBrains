using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.ViewModels;

namespace WebStore.Components
{
    public class SectionsViewComponent : ViewComponent
    {
        private readonly IProductService _prodService;

        public SectionsViewComponent(IProductService prodService)
        {
            _prodService = prodService;
        }

        public IViewComponentResult Invoke(string sectionId)
        {
            var sectId = int.TryParse(sectionId, out var id) ? id : (int?)null;


            var sections = _getSections(sectId, out var parentSectionId);

            return View(new SelectableSectionsViewModel
            {
                Sections = sections,
                SectionId = sectId,
                ParentSectionId = parentSectionId,
            });
        }

        //public async Task<IViewComponentResult> InvokeAsync() => View();  //можно и асинхронный 


        private IEnumerable<SectionViewModel> _getSections(int? sectionId, out int? parentSectionId)
        {
            parentSectionId = null;

            var sections = _prodService.GetSections();
            var parents = sections.Where(s => s.ParentId is null);

            var parentViews = parents
                .Select(p => new SectionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Order = p.Order,
                })
                .ToList();

            foreach (var pv in parentViews)
            {
                var childs = sections.Where(s => s.ParentId == pv.Id);

                foreach (var child in childs)
                {
                    if (child.Id == sectionId)
                    {
                        parentSectionId = child.ParentId;
                    }

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

            return parentViews;
        }
    }
}
