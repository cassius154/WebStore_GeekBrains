using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Services.Cookies
{
    public class CookiesCartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly string _cartName;

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
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Decrement(int id)
        {
            throw new NotImplementedException();
        }

        public CartViewModel GetViewModel()
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
