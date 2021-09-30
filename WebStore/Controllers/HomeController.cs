using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Cart() => View();

        public IActionResult Checkout() => View();

        public IActionResult ContactUs() => View();

        public IActionResult Login() => View();

        public IActionResult ProductDetails() => View();

        public IActionResult Shop() => View();

        public IActionResult Account() => View();

        public IActionResult Dreams() => View();

        public IActionResult NotFoundPage() => View();

        public IActionResult Status(string code) => Content($"Status code = {code}");
    }
}
