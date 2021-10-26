using WebStore.Domain.Entities;

namespace WebStore.Interfaces.Services
{
    //производим декомпозицию сервиса Cart
    //чтобы при тестировании один сервис отвечал исключительно за логику корзины
    //а другой за всю оболочку (HttpContext, Coockies и т.д.)
    public interface ICartStore
    {
        public Cart Cart { get; set; }
    }
}
