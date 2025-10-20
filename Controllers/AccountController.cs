using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.Models;
using System.Diagnostics;
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
                    else if(user.КодРолиNavigation.Название == "Менеджер")
                    {
                        return RedirectToAction("ManagerDashboard", "Account");
                    }
                    else if(user.КодРолиNavigation.Название == "Админ")
                    {
                        return RedirectToAction("AdminDashboard", "Account");
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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Debug.WriteLine($"User id from cookie: {userId}");
            foreach(var claim in User.Claims)
            {
                Debug.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }
            var user = await _context.Пользователи
                .Include(u=>u.Заказы)
                    .ThenInclude(z=>z.КодСтатусаЗаказаNavigation)
                .FirstOrDefaultAsync(u => u.КодПользователя == userId);
            if(user == null)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }
            user.Заказы = user.Заказы.OrderByDescending(z => z.ДатаСозданияЗаказа).ToList();

            return View(user);
        }

        [Authorize(Roles = "Менеджер")]
        public async Task<IActionResult> ManagerDashboard(int statusFilter, string sortFilter="newest")
        {

            var allStatuses = await _context.СтатусыЗаказа.ToListAsync();
            IQueryable<Заказы> orderQuery = _context.Заказы
                    .Include(z=>z.КодКлиентаNavigation)
                    .Include(z=>z.КодСтатусаЗаказаNavigation);
            if(statusFilter > 0)
            {
                orderQuery = orderQuery.Where(z=>z.КодСтатусаЗаказа == statusFilter); 
            }
            switch (sortFilter)
            {
                case "newest":
                    orderQuery = orderQuery.OrderByDescending(z => z.ДатаСозданияЗаказа);
                    break;
                case "oldest":
                default:
                    orderQuery = orderQuery.OrderBy(z => z.ДатаСозданияЗаказа);
                    break;
            }

            var orders = await orderQuery.ToListAsync();

            var viewModel = new ManagerDashboardViewModel
            {
                Заказы = orders,
                ВсеСтатусы = allStatuses.OrderBy(s => s.КодСтатусаЗаказа),
                ВыбранныйСтатусКод = statusFilter,
                Сортировка = sortFilter

            };

        
            return View(viewModel);
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
