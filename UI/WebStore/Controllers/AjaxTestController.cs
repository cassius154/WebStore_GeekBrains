using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebStore.ViewModels;

namespace WebStore.Controllers
{
    public class AjaxTestController : Controller
    {
        private readonly ILogger<AjaxTestController> _logger;

        public AjaxTestController(ILogger<AjaxTestController> logger) => _logger = logger;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetJSON(int? id, string msg, int delay = 2000)
        {
            _logger.LogInformation("Получен запрос к GetJSON - id:{id}, msg:{msg}, Delay:{Delay}", id, msg, delay);

            await Task.Delay(delay);

            _logger.LogInformation("Ответ на запрос к GetJSON - id:{id}, msg:{msg}, Delay:{Delay}", id, msg, delay);
            return Json(new
            {
                Message = $"Response (id:{id ?? 0}): {msg ?? "--null--"}",
                ServerTime = DateTime.Now,
            });
        }

        public async Task<IActionResult> GetHTML(int? id, string msg, int Delay = 2000)
        {
            _logger.LogInformation("Получен запрос к GetHTML - id:{id}, msg:{msg}, Delay:{Delay}", id, msg, Delay);

            await Task.Delay(Delay);

            _logger.LogInformation("Ответ на запрос к GetHTML - id:{id}, msg:{msg}, Delay:{Delay}", id, msg, Delay);

            return PartialView("Partial/_DataView", new AjaxTestDataViewModel
            {
                Id = id ?? 0,
                Message = msg,
            });
        }
    }
}
