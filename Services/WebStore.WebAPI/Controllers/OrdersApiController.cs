using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.DTO;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersApiController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("user/{UserName}")]
        public async Task<IActionResult> GetUserOrders(string userName)
        {
            var orders = await _orderService.GetUserOrders(userName);
            return Ok(orders.ToDTO());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            //почему в контроллерах нельзя вызывать ConfigureAwait(false)
            //ControllerContext.HttpContext - здесь он есть
            var order = await _orderService.GetOrderById(id);  //если тут вызвать ConfigureAwait(false)
            //ControllerContext.HttpContext - то в новом потоке он будет сброшен и мы останемся без контекста
            //контекст здесь нужно сохранить при переходе через await
            return order is null ? NotFound() : Ok(order.ToDTO());
        }

        [HttpPost("{UserName}")]
        public async Task<IActionResult> CreateOrder(string userName, [FromBody] CreateOrderDTO orderModel)
        {
            var order = await _orderService.CreateOrder(userName, orderModel.Items.ToCartView(), orderModel.Order);
            return Ok(order.ToDTO());
        }
    }
}
