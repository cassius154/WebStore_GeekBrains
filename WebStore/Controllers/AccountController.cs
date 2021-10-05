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
                await _userManager.AddToRoleAsync(user, Role.Users);  //добавляем сразу в группу Users

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in regResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        #endregion Register

        #region Login
        public IActionResult Login(string returnUrl) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginResult = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                false);

            if (loginResult.Succeeded)
            {
                //return Redirect(model.ReturnUrl);  //небезопасно! могут подсунуть левый Url и куки утекут на сторону

                //можно так
                //if (Url.IsLocalUrl(model.ReturnUrl))
                //{
                //    return Redirect(model.ReturnUrl);
                //}
                //return RedirectToAction("Index", "Home");

                //а можно так
                return LocalRedirect(model.ReturnUrl ?? "/");
            }

            ModelState.AddModelError("", "Ошибка ввода логина или пароля");
            return View(model);
        }
        #endregion Login

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
