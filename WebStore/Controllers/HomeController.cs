﻿using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Hello from HomeController!");
            //return View();
        }

        public IActionResult SecondAction(int id)
        {
            return Content($"Hello from SecondAction with id = {id}!");
        }
    }
}
