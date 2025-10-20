using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.Models;

namespace OKNODOM.Controllers
{
    [Authorize(Roles = "Менеджер")]
    public class ManagerController : Controller
    {
        private readonly OknodomDbContext _context;
        public ManagerController(OknodomDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Заказы
                .Include(z => z.КодКлиентаNavigation)
                .Include(z => z.КодСтатусаЗаказаNavigation)
                .Include(z => z.Замеры)
                .FirstOrDefaultAsync(z => z.КодЗаказа == id);
            if (order == null) return NotFound();   

            var measurers = await _context.Пользователи
                .Where(u=>u.КодРоли == 3)
                .OrderBy(u=> u.Фамилия)
                .ToListAsync();

            var viewModel = new OrderDetailsManagerViewModel
            {
                Заказ = order,
                Замерщики = measurers
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMeasurer(int orderId, int selectedMeasurerId)
        {
            var order = await _context.Заказы
                .FirstOrDefaultAsync(z=>z.КодЗаказа ==  orderId);

            if (order == null || order.КодСтатусаЗаказа !=1) 
            {
                TempData["ErrorMessage"] = "Невозможно назначить замерщика";
                return RedirectToAction("ManagerDashboard", "Account");
            } 
            var newMeasurement = new Замеры
            {
                КодЗаказа = orderId,
                КодЗамерщика = selectedMeasurerId,
                ДатаЗамера = DateTime.Now
            };
         
            order.КодСтатусаЗаказа = 2; //статус назначенного замера

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Замерщик успешно назначен на заказ №{orderId}. Статус обновлен";
            return RedirectToAction("OrderDetails", new {id=orderId});
        }
        
    }
}
