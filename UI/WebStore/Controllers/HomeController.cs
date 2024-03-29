﻿using System;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Exception(string Message) => throw new InvalidOperationException(Message ?? "Ошибка в контроллере!");

        public IActionResult Cart() => View();

        public IActionResult Checkout() => View();

        public IActionResult ContactUs() => View();

        public IActionResult Login() => View();

        public IActionResult ProductDetails() => View();

        public IActionResult Shop() => View();

        public IActionResult Account() => View();

        public IActionResult Dreams() => View();

        public IActionResult NotFoundPage() => View();

        public IActionResult Status(string id)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            switch (id)
            {
                default: return Content($"Status code === {id}");
                case "404": return View("NotFoundPage");
            }
        }
    }
}
