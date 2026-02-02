using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.DTOs.MeasurerDto;

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
                        new Claim(ClaimTypes.Role, user.КодРолиNavigation.Название)
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
                    else if(user.КодРолиNavigation.Название == "Замерщик")
                    {
                        return RedirectToAction("MeasurerDashboard", "Account");
                    }
                    else if(user.КодРолиNavigation.Название == "Монтажник")
                    {
                        return RedirectToAction("InstallerDashboard", "Account");
                    }

                     return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            }
            userModel.Пароль = string.Empty;
            return View(userModel);
        }


        [Authorize(Roles="Клиент")]
        public async Task<IActionResult> ClientDashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _context.Пользователи
                .FirstOrDefaultAsync(u => u.КодПользователя == userId);
            if(user == null)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }

            var orders = await _context.Заказы
                .Where(z=>z.КодКлиента == userId)
                .Include(z=>z.КодСтатусаЗаказаNavigation)
                .OrderByDescending(z=>z.ДатаСозданияЗаказа)
                .ToListAsync();
            user.Заказы = orders.ToList();
                
            return View(user);
        }

        [Authorize(Roles = "Менеджер")]
        public async Task<IActionResult> ManagerDashboard(int statusFilter, string sortFilter="newest", string search = "")
        {

            var allStatuses = await _context.СтатусыЗаказа.ToListAsync();
            IQueryable<Заказы> orderQuery = _context.Заказы
                    .Include(z=>z.КодКлиентаNavigation)
                    .Include(z=>z.КодСтатусаЗаказаNavigation);
            if(statusFilter > 0)
            {
                orderQuery = orderQuery.Where(z=>z.КодСтатусаЗаказа == statusFilter); 
            }
            // Фильтр по поиску (адрес или ФИО)
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower().Trim();
                orderQuery = orderQuery.Where(z =>
                    z.Адрес.ToLower().Contains(search) ||
                    z.КодКлиентаNavigation.Фамилия.ToLower().Contains(search) ||
                    z.КодКлиентаNavigation.Имя.ToLower().Contains(search) ||
                    z.КодКлиентаNavigation.Отчество.ToLower().Contains(search) ||
                    (z.КодКлиентаNavigation.Фамилия + " " +
                     z.КодКлиентаNavigation.Имя + " " +
                     z.КодКлиентаNavigation.Отчество).ToLower().Contains(search));
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
                Сортировка = sortFilter,
                Поиск = search

            };    
            return View(viewModel);
        }

        [Authorize(Roles = "Замерщик")]
        public async Task<IActionResult> MeasurerDashboard(DateTime? dateFrom = null, DateTime? dateTo = null, string tab = "active")
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var activeQuery = _context.Замеры
                 .Where(z => z.КодЗамерщика == userId
                         && z.КодЗаказаNavigation.КодСтатусаЗаказа == 2);

            var completedQuery = _context.Замеры
                .Include(z => z.КодЗаказаNavigation)
                .Where(z => z.КодЗамерщика == userId
                        && z.КодЗаказаNavigation.КодСтатусаЗаказа > 2);

            if (dateFrom.HasValue)
            {
                activeQuery = activeQuery.Where(z => z.ДатаЗамера >= dateFrom.Value);
                completedQuery = completedQuery.Where(z => z.ДатаЗамера >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                var endOfDays = dateTo.Value.AddDays(1);
                activeQuery = activeQuery.Where(z => z.ДатаЗамера < endOfDays);
                completedQuery = completedQuery.Where(z=>z.ДатаЗамера < endOfDays);
            }

            IQueryable<Замеры> currentQuery = tab == "active" ? activeQuery : completedQuery;

            var orders = await currentQuery
                .OrderByDescending(z => z.ДатаЗамера)
                .Select(z => new MeasurerOrderViewModel
                {
                    КодЗаказа = z.КодЗаказа,
                    Адрес = z.КодЗаказаNavigation.Адрес,
                    ФиоКлиента = z.КодЗаказаNavigation.КодКлиентаNavigation.Фамилия + " " +
                         z.КодЗаказаNavigation.КодКлиентаNavigation.Имя,
                    Телефон = z.КодЗаказаNavigation.КодКлиентаNavigation.Телефон,
                    Статус = z.КодЗаказаNavigation.КодСтатусаЗаказаNavigation.Название,
                    ДатаЗамера = z.ДатаЗамера
                })
              .ToListAsync();


            var viewModel = new MeasurerDashboardViewModel
            {
                Заказы = orders,
                ДатаС = dateFrom,
                ДатаПо = dateTo,
                АктивнаяВкладка = tab
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Монтажник")]
        public async Task<IActionResult> InstallerDashboard(string tab = "active", int? orderId = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // 1. Создаем базовый запрос
            IQueryable<Бригады> query = _context.Бригады
                .Where(b => b.КодМонтажника == userId);

            // 2. Применяем фильтр по статусу
            if (tab == "active")
            {
                query = query.Where(b =>
                    b.КодВыполненияNavigation.КодТовараВЗаказеNavigation
                     .КодЗаказаNavigation.КодСтатусаЗаказа == 5);
            }
            else
            {
                query = query.Where(b =>
                    b.КодВыполненияNavigation.КодТовараВЗаказеNavigation
                     .КодЗаказаNavigation.КодСтатусаЗаказа == 6);
            }

            // 3. Только ПОСЛЕ фильтрации добавляем Include
            query = query
                .Include(b => b.КодВыполненияNavigation)
                    .ThenInclude(v => v.КодТовараВЗаказеNavigation)
                        .ThenInclude(t => t.КодЗаказаNavigation)
                            .ThenInclude(z => z.КодСтатусаЗаказаNavigation)
                .Include(b => b.КодВыполненияNavigation)
                    .ThenInclude(v => v.КодТовараВЗаказеNavigation)
                        .ThenInclude(t => t.КодЗаказаNavigation)
                            .ThenInclude(z => z.КодКлиентаNavigation);

            var бригады = await query.ToListAsync();
          
            // Группируем по заказу (чтобы не дублировать)
            var заказы = бригады
                .GroupBy(b => b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказа)
                .Select(g => g.First())
                .Select(b => new InstallerOrderViewModel
                {
                    КодЗаказа = b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказа,
                    Адрес = b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказаNavigation.Адрес,
                    ФиоКлиента = b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказаNavigation.КодКлиентаNavigation.Фамилия + " " +
                                 b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказаNavigation.КодКлиентаNavigation.Имя + " " +
                                  b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказаNavigation.КодКлиентаNavigation.Отчество,
                    Телефон = b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказаNavigation.КодКлиентаNavigation.Телефон,
                    Статус = b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказаNavigation.КодСтатусаЗаказаNavigation.Название,
                    ДатаНазначения = b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказаNavigation.ДатаСозданияЗаказа
                })
                .ToList();

            var viewModel = new InstallerDashboardViewModel
            {
                Заказы = заказы,
                АктивнаяВкладка = tab
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
