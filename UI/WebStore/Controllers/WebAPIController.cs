using Microsoft.AspNetCore.Mvc;
using WebStore.Interfaces.TestAPI;

namespace WebStore.Controllers
{
    public class WebAPIController : Controller
    {
        private readonly IValuesClient _valuesClient;

        public WebAPIController(IValuesClient valuesClient) => _valuesClient = valuesClient;

        public IActionResult Index()
        {
            var values = _valuesClient.GetAll();
            //тут тест провалится, поскольку мы там проверяем valuesClientMock.VerifyNoOtherCalls();
            //(ничего кроме GetAll())
            //var c = _valuesClient.Count();
            return View(values);
        }
    }
}
