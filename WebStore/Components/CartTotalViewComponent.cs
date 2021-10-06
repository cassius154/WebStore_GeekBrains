using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebStore.Services.Interfaces;

namespace WebStore.Components
{
    public class CartTotalViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;

        public CartTotalViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_cartService.GetViewModel());
        }
    }
}
