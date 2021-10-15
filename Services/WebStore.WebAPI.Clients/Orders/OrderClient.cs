using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebStore.Domain.DTO;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Orders
{
    public class OrderClient : ClientBase, IOrderService
{
        public OrderClient(HttpClient client) : base(client, "api/orders") { }

        public async Task<IEnumerable<Order>> GetUserOrders(string userName)
        {
            var orders = await GetAsync<IEnumerable<OrderDTO>>($"{address}/user/{userName}")
                .ConfigureAwait(false);
            return orders.FromDTO();
        }

        public async Task<Order> GetOrderById(int id)
        {
            var order = await GetAsync<OrderDTO>($"{address}/{id}").ConfigureAwait(false);
            return order.FromDTO();
        }

        public async Task<Order> CreateOrder(string userName, CartViewModel cart, OrderViewModel orderModel)
        {
            var model = new CreateOrderDTO
            {
                Items = cart.ToDTO(),
                Order = orderModel,
            };

            var response = await PostAsync($"{address}/{userName}", model).ConfigureAwait(false);
            var order = await response
               .EnsureSuccessStatusCode()
               .Content
               .ReadFromJsonAsync<OrderDTO>()
               .ConfigureAwait(false);

            return order.FromDTO();
        }
    }
}
