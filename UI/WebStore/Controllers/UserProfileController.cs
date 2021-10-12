using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        public IActionResult Index() => View();

        public async Task<IActionResult> Orders([FromServices] IOrderService orderService)
        {
            var orders = await orderService.GetUserOrders(User.Identity.Name);

            return View(orders.Select(order => new UserOrderViewModel
            {
                Id = order.Id,
                Address = order.Address,
                Phone = order.Phone,
                Description = order.Description,
                Total = order.Total,
                Date = order.Date,
            }));
        }
    }
}
