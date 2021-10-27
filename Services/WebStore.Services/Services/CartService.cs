using System.Linq;
using WebStore.Domain;
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
            var cart = _cartStore.Cart;  //берем из входных кукисов

            var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);
            if (item is null)
            {
                cart.Items.Add(new CartItem { ProductId = Id, Quantity = 1 });
            }
            else
            {
                item.Quantity++;
            }

            _cartStore.Cart = cart;  //записываем обратно в выходные кукисы
        }

        public void Decrement(int Id)
        {
            var cart = _cartStore.Cart;  //берем из входных кукисов

            var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);
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

            _cartStore.Cart = cart;  //записываем обратно в выходные кукисы
        }

        public void Remove(int Id)
        {
            var cart = _cartStore.Cart;  //берем из входных кукисов

            var item = cart.Items.FirstOrDefault(i => i.ProductId == Id);
            if (item is null)
            {
                return;
            }

            cart.Items.Remove(item);

            _cartStore.Cart = cart;  //записываем обратно в выходные кукисы
        }

        public void Clear()
        {
            var cart = _cartStore.Cart;  //берем из входных кукисов
            cart.Items.Clear();
            _cartStore.Cart = cart;  //записываем обратно в выходные кукисы

            //или можно было бы так, но возможны проблемы
            //_CartStore.Cart = new();
        }

        public CartViewModel GetViewModel()
        {
            var products = _productService.GetProducts(new ProductFilter
            {
                Ids = _cartStore.Cart.Items.Select(item => item.ProductId).ToArray()
            });

            var productsViewDict = products.ToProductView().ToDictionary(p => p.Id);

            return new CartViewModel
            {
                Items = _cartStore.Cart.Items
                   .Where(item => productsViewDict.ContainsKey(item.ProductId))
                   .Select(item => (productsViewDict[item.ProductId], item.Quantity))
            };
        }
    }
}
