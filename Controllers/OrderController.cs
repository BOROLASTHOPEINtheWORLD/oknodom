using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OKNODOM.DTOs;
using OKNODOM.Models;
using System.Security.Claims;

namespace OKNODOM.Controllers
{
    public class OrderController : Controller
    {
        private readonly OknodomDbContext _context;

        public OrderController(OknodomDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var userId= int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Пользователи.FindAsync(userId);
            if (user == null)
            {
                // Если пользователь найден в куки, но удален из БД
                return RedirectToAction("Login", "Account");
            }
            var model = new OrderViewModel
            {
                КодПользователя = user.КодПользователя,
                ПолноеИмя = $"{user.Фамилия} {user.Имя} {user.Отчество}",
                Телефон = user.Телефон,
                Адрес = ""
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newOrder = new Заказы
                {
                    КодКлиента = model.КодПользователя!.Value,
                    Адрес = model.Адрес,
                    ДатаСозданияЗаказа = DateTime.Now,
                    КодСтатусаЗаказа = 1,
                    СтатусОплаты = false,
                    ПримечаниеКЗаказу = model.Описание
                };
                _context.Заказы.Add(newOrder);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ваша заявка успешно создана и принята в обработку!";
                return RedirectToAction("ClientDashboard", "Account");
            }
            return View(model);
        }
    }
}
