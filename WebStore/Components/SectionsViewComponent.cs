using Microsoft.AspNetCore.Mvc;

namespace WebStore.Components
{
    public class SectionsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke() => View();

        //public async Task<IViewComponentResult> InvokeAsync() => View();  //можно и асинхронный 
    }
}
