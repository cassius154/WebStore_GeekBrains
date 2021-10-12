using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;

namespace WebStore.Components
{
    public class CartTotalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CartViewModel model)
        {
            return View(model);
        }
    }
}
