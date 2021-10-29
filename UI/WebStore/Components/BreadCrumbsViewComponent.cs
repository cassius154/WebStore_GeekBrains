using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Components
{
    public class BreadCrumbsViewComponent : ViewComponent
    {
        private readonly IProductService _productService;

        public BreadCrumbsViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public IViewComponentResult Invoke()
        {
            var model = new BreadCrumbsViewModel();

            if (int.TryParse(Request.Query["SectionId"], out var sectionId))
            {
                model.Section = _productService.GetSectionById(sectionId);
                if (model.Section.ParentId is { } parentSectionId)
                    model.Section.Parent = _productService.GetSectionById(parentSectionId);
            }

            if (int.TryParse(Request.Query["BrandId"], out var brandId))
            {
                model.Brand = _productService.GetBrandById(brandId);
            }

            if (int.TryParse(ViewContext.RouteData.Values["id"]?.ToString(), out var productId))
            {
                model.Product = _productService.GetProductById(productId)?.Name;
            }

            return View(model);
        }
    }
}
