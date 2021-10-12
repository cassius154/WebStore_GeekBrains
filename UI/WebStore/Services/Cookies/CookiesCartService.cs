using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;
using WebStore.DTO;
using WebStore.Services.Interfaces;

namespace WebStore.Services.Cookies
{
    public class CookiesCartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly string _cartName;

        private Cart _cart
        {
            get  //здесь идет десериализация
            {
                var context = _httpContextAccessor.HttpContext;
                var cookies = context.Response.Cookies;

                var cartCookie = context.Request.Cookies[_cartName];
                if (cartCookie is null)
                {
                    //если не нашли - создаем новую корзину
                    var cart = new Cart();
                    cookies.Append(_cartName, JsonConvert.SerializeObject(cart));  //записываем ее сериализованную в кукисы РЕСПОНЗА
                    return cart;  //и ее же возвращаем
                }

                _replaceCart(cookies, cartCookie);  //по сути - переписываем куку из request в response
                return JsonConvert.DeserializeObject<Cart>(cartCookie);  //и возвращаем ее десериализованную
            }
            set  //здесь сериализация
            {
                _replaceCart(_httpContextAccessor.HttpContext!.Response.Cookies, 
                    JsonConvert.SerializeObject(value));
            }
        }

        private void _replaceCart(IResponseCookies cookies, string cart)
        {
            cookies.Delete(_cartName);
            cookies.Append(_cartName, cart);
        }

        public CookiesCartService(IHttpContextAccessor httpContextAccessor, IProductService productService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;

            var user = httpContextAccessor.HttpContext!.User;
            var userName = user.Identity!.IsAuthenticated ? $"-{user.Identity!.Name}" : null;
            _cartName = $"GB.WebStore.Cart{userName}";
        }

        public void Add(int id)
        {
            var cart = _cart;  //берем из входных кукисов

            var item = cart.Items.FirstOrDefault(i => i.ProductId == id);
            if (item is null)
            {
                cart.Items.Add(new CartItem { ProductId = id, Quantity = 1 });
            }
            else
            {
                item.Quantity++;
            }

            _cart = cart;  //записываем обратно в выходные кукисы
        }

        public void Clear()
        {
            var cart = _cart;  //берем из входных кукисов
            cart.Items.Clear();
            _cart = cart;  //записываем обратно в выходные кукисы

            //или можно было бы так, но возможны проблемы
            //_cart = new();
        }

        public void Decrement(int id)
        {
            var cart = _cart;  //берем из входных кукисов

            var item = cart.Items.FirstOrDefault(i => i.ProductId == id);
            if (item is null)
            {
                return;
            }
            
            if (item.Quantity > 0)
            {
                item.Quantity--;
            }

            if (item.Quantity <= 0)
            {
                cart.Items.Remove(item);
            }

            _cart = cart;  //записываем обратно в выходные кукисы
        }

        public void Remove(int id)
        {
            var cart = _cart;  //берем из входных кукисов

            var item = cart.Items.FirstOrDefault(i => i.ProductId == id);
            if (item is null)
            {
                return;
            }
            cart.Items.Remove(item);

            _cart = cart;  //записываем обратно в выходные кукисы
        }

        public CartViewModel GetViewModel()
        {
            var products = _productService.GetProducts(new ProductFilter
            {
                Ids = _cart.Items.Select(i => i.ProductId).ToArray()
            });
            var productViewDict = products.ToProductView().ToDictionary(p => p.Id);

            return new CartViewModel
            {
                Items = _cart.Items
                    .Where(item => productViewDict.ContainsKey(item.ProductId))  //здесь мы отсекаем возможные левые Id из корзины, и оставляем только те, которые реально вытащили из БД
                    .Select(item => (productViewDict[item.ProductId], item.Quantity)),
            };
        }
    }
}
