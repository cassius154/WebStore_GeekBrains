using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.Identity;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Services.Services.SQL
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

        public async Task<Order> CreateOrder(string userName, CartViewModel cart, OrderViewModel orderModel)
        {
            //ConfigureAwait(false) надо вызывать только при первом асинхронном вызове - дальше необязательно. (ПРОВЕРИТЬ!!!)
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
            {
                throw new InvalidOperationException($"Пользователь {userName} не найден");
            }

            //новый синтаксис - без блока {}
            //await using (var tx = await _db.Database.BeginTransactionAsync()) {}
            await using var tx = await _db.Database.BeginTransactionAsync();

            //создаем Order
            var order = new Order
            { 
                User = user,
                Address = orderModel.Address,
                Phone = orderModel.Phone,
                Description = orderModel.Description,
            };


            //создаем Items
            //берем id-шники продуктов из корзины
            var pIds = cart.Items.Select(i => i.Product.Id);
            //по этим id-шникам тянем продукты из базы
            var cartProducts = await _db.Products
                .Where(p => pIds.Contains(p.Id))
                .ToArrayAsync();
            //джойним продукты из корзины с продуктами из базы по Id
            order.Items = cart.Items.Join(
                cartProducts,
                cartItem => cartItem.Product.Id,  //продукты из корзины
                cartProduct => cartProduct.Id,    //продукты из БД - джойним по Id
                (cartItem, cartProduct) => new OrderItem
                {
                    Order = order,  //созданный order
                    Product = cartProduct,
                    Price = cartProduct.Price,     //цена из БД - тут можно добавить скидку
                    Quantity = cartItem.Quantity,  //кол-во из корзины
                }).ToArray();

            await _db.Orders.AddAsync(order);
            //await _db.Set<OrderItem>().AddRangeAsync(order.Items);  //нет необходимости - _db.Orders.AddAsync(order) сам все добавит
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            return order;
        }

        public async Task<Order> GetOrderById(int id)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id)
                //если мы пришли сюда в потоке thread.id = 7,
                //без ConfigureAwait(false) будем дожидаться thread,
                //который нас сюда привел (id = 7)
                //с ConfigureAwait(false) будем брать любой thread
                .ConfigureAwait(false);
            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrders(string userName)
        {
            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.User.UserName == userName)
                .ToArrayAsync()
                .ConfigureAwait(false);
            return orders;
        }
    }
}
