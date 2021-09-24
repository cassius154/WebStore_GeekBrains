using Microsoft.AspNetCore.Mvc;

namespace WebStore.Components
{
    //[ViewComponent(Name = "BrandsVvv")]  - вместо наследования можно применить этот атрибут
    public class BrandsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke() => View();

        //public async Task<IViewComponentResult> InvokeAsync() => View();  //можно и асинхронный 
    }
}
