using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.Identity;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Services.SQL
{
    public class DBOrderService : IOrderService
    {
        private readonly WebStoreDbContext _db;
        private readonly UserManager<User> _userManager;

        public DBOrderService(WebStoreDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public Task<Order> CreateOrder(string userName, CartViewModel cart, OrderViewModel order)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetUserOrder(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
