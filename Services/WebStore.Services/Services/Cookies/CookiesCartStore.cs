using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.Services.Services.Cookies
{
    public class CookiesCartStore : ICartStore
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _cartName;

        public CookiesCartStore(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            var user = httpContextAccessor.HttpContext!.User;
            var user_name = user.Identity!.IsAuthenticated ? $"-{user.Identity.Name}" : null;

            _cartName = $"GB.WebStore.Cart{user_name}";
        }

        public Cart Cart
        {
            get  //здесь идет десериализация
            {
                var context = _httpContextAccessor.HttpContext;
                //берем кукисы РЕСПОНЗА
                var cookies = context!.Response.Cookies;

                //ищем куку корзины в кукисах РЕКВЕСТА
                var cartCookie = context.Request.Cookies[_cartName];
                if (cartCookie is null)
                {
                    //если не нашли - создаем новую корзину
                    var cart = new Cart();
                    //записываем ее сериализованную в кукисы РЕСПОНЗА
                    cookies.Append(_cartName, JsonConvert.SerializeObject(cart));
                    return cart;  //и ее же возвращаем в несериализованном виде
                }

                //если нашли - заменяем в кукисах РЕСПОНЗА
                _replaceCart(cookies, cartCookie);
                //и возвращаем ДЕСЕРИАЛИЗОВАННУЮ
                return JsonConvert.DeserializeObject<Cart>(cartCookie);
            }
            set  //здесь сериализация и запись в кукисы РЕСПОНЗА
            {
                //сериализуем и заменяем в кукисах РЕСПОНЗА
                _replaceCart(_httpContextAccessor.HttpContext!.Response.Cookies,
                    JsonConvert.SerializeObject(value));
            }
        }

        private void _replaceCart(IResponseCookies cookies, string cart)
        {
            cookies.Delete(_cartName);
            cookies.Append(_cartName, cart);
        }
    }
}
