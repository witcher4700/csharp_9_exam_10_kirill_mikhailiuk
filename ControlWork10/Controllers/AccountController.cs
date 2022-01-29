using ControlWork10.Models;
using ControlWork10.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ControlWork10.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private ApplicationDbContext _context;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.UserName,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                //Создание пользователя средствами Identity на основе объекта пользователя и его пароля
                //Возвращает результат выполнения в случае успеха впускаем пользователя в систему
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Place");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    false
                    );
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Place");
                }
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Place");
        }

        public IActionResult CheckData(string name, string email)
        {
            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                if(user.Email == email)
                    return Ok("Такой Email уже существует, используйте другой");
                if (user.UserName == name)
                    return Ok("Такое имя пользователя уже существует, используйте дрогое");
            }
            return Ok(200);
        }
        
    }
}
