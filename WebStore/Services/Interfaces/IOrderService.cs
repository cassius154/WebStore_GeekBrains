using System.Collections.Generic;
using System.Threading.Tasks;
using WebStore.Domain.Entities.Orders;
using WebStore.ViewModels;

namespace WebStore.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetUserOrder(string userName);

        Task<Order> GetOrderById(int id);

        Task<Order> CreateOrder(string userName, CartViewModel cart, OrderViewModel order);
    }
}
