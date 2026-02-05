using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.Models;
using OKNODOM.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OKNODOM.Controllers
{
    [Authorize(Roles = "Клиент")]
    public class ClientController : Controller
    {
        private readonly OknodomDbContext _context;
        private readonly OrderDetailsService _orderDetailsService;

        public ClientController(OknodomDbContext context, OrderDetailsService orderDetailsService)
        {
            _context = context;
            _orderDetailsService = orderDetailsService;
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            // Проверяем доступ
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Forbid();

            // Проверяем принадлежность заказа
            var orderExists = await _context.Заказы
                .AnyAsync(z => z.КодЗаказа == id && z.КодКлиента == userId);

            if (!orderExists) return Forbid();

            // Используем общий сервис
            var viewModel = await _orderDetailsService.BuildOrderDetailsViewModel(id);

            if (viewModel == null) return NotFound();

            return View(viewModel);
        }

        // ДОБАВЬ ЭТОТ МЕТОД
        [HttpPost]
        public async Task<IActionResult> PayOrder(int id)
        {
            // Получаем ID пользователя
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Forbid();

            // Находим заказ пользователя
            var order = await _context.Заказы
                .FirstOrDefaultAsync(o => o.КодЗаказа == id && o.КодКлиента == userId);

            if (order == null)
                return NotFound();

            // Просто меняем статус оплаты
            order.СтатусОплаты = true;
            await _context.SaveChangesAsync();

            // Перенаправляем обратно на страницу заказа
            return RedirectToAction("OrderDetails", new { id });
        }
    }
}