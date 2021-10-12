using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index() => View(new CartOrderViewModel { Cart = _cartService.GetViewModel(), });

        public IActionResult Add(int id)
        {
            _cartService.Add(id);

            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Decrement(int id)
        {
            _cartService.Decrement(id);

            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Remove(int id)
        {
            _cartService.Remove(id);

            return RedirectToAction("Index", "Cart");
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(OrderViewModel orderModel, 
            //т.к. нужен всего в одном месте - используем [], а не передаем в конструктор
            [FromServices] IOrderService orderService)
        {
            var cart = _cartService.GetViewModel();
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), new CartOrderViewModel
                {
                    Cart = cart,
                    Order = orderModel,
                });
            }

            var order = await orderService.CreateOrder(
                User.Identity.Name,
                cart,
                orderModel);
            
            _cartService.Clear();
            
            return RedirectToAction(nameof(OrderConfirmed), new { id = order.Id });
        }

        public IActionResult OrderConfirmed(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
    }
}
