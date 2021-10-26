using System.Linq;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Services.DTO;


namespace WebStore.Services.Services
{
    public class CartService : ICartService
    {
        private readonly ICartStore _cartStore;
        private readonly IProductService _productService;

        public CartService(ICartStore cartStore, IProductService productService)
        {
            _cartStore = cartStore;
            _productService = productService;
        }

        public void Add(int Id)
        {
            var cart = _cartStore.Cart;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);
            if (item is null)
                cart.Items.Add(new CartItem { ProductId = Id, Quantity = 1 });
            else
                item.Quantity++;

            _cartStore.Cart = cart;
        }

        public void Decrement(int Id)
        {
            var cart = _cartStore.Cart;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);
            if (item is null) return;

            if (item.Quantity > 0)
                item.Quantity--;

            if (item.Quantity <= 0)
                cart.Items.Remove(item);

            _cartStore.Cart = cart;
        }

        public void Remove(int Id)
        {
            var cart = _cartStore.Cart;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);
            if (item is null) return;

            cart.Items.Remove(item);

            _cartStore.Cart = cart;
        }

        public void Clear()
        {
            //_CartStore.Cart = new();

            var cart = _cartStore.Cart;
            cart.Items.Clear();
            _cartStore.Cart = cart;
        }

        public CartViewModel GetViewModel()
        {
            var products = _productService.GetProducts(new()
            {
                Ids = _cartStore.Cart.Items.Select(item => item.ProductId).ToArray()
            });

            var products_views = products.ToProductView().ToDictionary(p => p.Id);

            return new CartViewModel
            {
                Items = _cartStore.Cart.Items
                   .Where(item => products_views.ContainsKey(item.ProductId))
                   .Select(item => (products_views[item.ProductId], item.Quantity))
            };
        }
    }
}
