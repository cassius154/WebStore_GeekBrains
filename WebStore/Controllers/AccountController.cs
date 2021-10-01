using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Identity;
using WebStore.ViewModels.Identity;

namespace WebStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Register
        public IActionResult Register() => View();

        //AntiForgeryToken - к форме прикрепляется некая информация, которую заранее передает сервер
        //(в методе GET, если указать во вьюхе на форме атрибут у tag-helpera form asp-antiforgery="true")
        //потом этот токен возвращается с формой в POST - и можно подтвердить, что не было подмены агента (браузера)
        //не было перехвата информации и ее подмены
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
            };

            var regResult = await _userManager.CreateAsync(user, model.Password);
            if (regResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, false); //false - непостоянный, разовый вход
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in regResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        #endregion Register

        public IActionResult Login() => View();

        public async Task<IActionResult> Logout()
        {
            
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
