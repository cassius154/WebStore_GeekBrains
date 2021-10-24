using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration;
using WebStore.Domain.Identity;
using WebStore.ViewModels.Identity;

namespace WebStore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;


        public AccountController(UserManager<User> userManager, 
            SignInManager<User> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;   // WebStore.Controllers.AccountController  - источник записи в логе будет такой
        }

        #region Register

        [AllowAnonymous]
        public IActionResult Register() => View();

        //AntiForgeryToken - к форме прикрепляется некая информация, которую заранее передает сервер
        //(в методе GET, если указать во вьюхе на форме атрибут у tag-helpera form asp-antiforgery="true")
        //потом этот токен возвращается с формой в POST - и можно подтвердить, что не было подмены агента (браузера)
        //не было перехвата информации и ее подмены
        [HttpPost, ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (_logger.BeginScope("Регистрация пользователя {UserName}", model.UserName))
            {
                var user = new User
                {
                    UserName = model.UserName,
                };

                //_logger.LogInformation("Регистрация пользователя {0}", user.UserName);
                _logger.LogInformation("Регистрация пользователя {UserName}", user.UserName); //можно так
                                                                                              //_logger.LogInformation($"Регистрация пользователя {user.UserName}"); // так не делать!
                var regResult = await _userManager.CreateAsync(user, model.Password);
                if (regResult.Succeeded)
                {
                    _logger.LogInformation("Пользователь {0} успешно зарегистрирован", user.UserName);

                    await _userManager.AddToRoleAsync(user, Role.Users);  //добавляем сразу в группу Users
                    _logger.LogInformation("Пользователю {0} назначена роль {1}", user.UserName, Role.Users);

                    await _signInManager.SignInAsync(user, false); //false - непостоянный, разовый вход
                    _logger.LogInformation("Пользователь {0} вошёл в систему после регистрации", user.UserName);

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in regResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                _logger.LogWarning("Ошибка при регистрации пользователя {0}: {1}",
                        user.UserName, string.Join(", ", regResult.Errors.Select(err => err.Description)));
            }

            return View(model);
        }
        #endregion Register

        #region Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost, ValidateAntiForgeryToken]
        [AllowAnonymous]
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
                _logger.LogInformation("Пользователь {0} успешно вошёл в систему", model.UserName);
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
            _logger.LogWarning("Ошибка ввода пользователя, или пароля при входе {0}", model.UserName);

            return View(model);
        }
        #endregion Login

        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity!.Name;
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Пользователь {0} вышел из системы", userName);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Пользователю {0} отказано в доступе к uri:{1}",
                User.Identity!.Name, HttpContext.Request.Path);
            return View();
        }
    }
}
