using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return View(values);
        }
    }
}
