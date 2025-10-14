using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.Models;
using System.Security.Claims;

namespace OKNODOM.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly OknodomDbContext _context;

        public AccountController(ILogger<AccountController> logger, OknodomDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel userModel)
        {
            if(ModelState.IsValid)
            {
                var user = await _context.Пользователи
                    .Include(u=>u.КодРолиNavigation)
                    .FirstOrDefaultAsync(u => u.Логин == userModel.Логин && u.Пароль == userModel.Пароль);
                if (user != null)
                {
                    var claims = new List<Claim> //список клеймов, где клейм это информация о пользователе для сессии
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.КодПользователя.ToString()),
                        new Claim(ClaimTypes.Name, user.Логин),
                        new Claim(ClaimTypes.Role, user.КодРолиNavigation.Название.ToString())
                     };

                    var claimIdentity= new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = userModel.ЗапомнитьМеня,
                        ExpiresUtc = userModel.ЗапомнитьМеня
                            ? DateTimeOffset.UtcNow.AddDays(30)
                            : DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimIdentity),
                        authProperties); //сохраняем в куки

                    if(user.КодРолиNavigation.Название == "Клиент")
                    {
                        return RedirectToAction("ClientDashboard", "Account");
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            }
            userModel.Пароль = string.Empty;
            return View(userModel);
        }
        [Authorize(Roles = "Клиент")]
        public async Task<IActionResult> ClientDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Пользователи.FirstOrDefaultAsync(u => u.КодПользователя.ToString() == userId);
            return View(user);
        }
        [HttpPost] 
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //очистка куки
            return RedirectToAction("Index", "Home");
        }
    }
}
