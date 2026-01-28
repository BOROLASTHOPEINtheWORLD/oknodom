using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs.MeasurerDto;
using OKNODOM.Models;
using System.Security.Claims;

namespace OKNODOM.Controllers
{
    [Authorize(Roles = "Замерщик")]
    public class MeasurerController : Controller
    {
        private readonly OknodomDbContext _context;

        public MeasurerController(OknodomDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MeasureDetails(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var model = await GetMeasureDetailsViewModel(id, userId);
                var order = await _context.Заказы.FirstOrDefaultAsync(z => z.КодЗаказа == id);

                ViewBag.МожноРедактировать = order?.КодСтатусаЗаказа == 2;

                return View(model);
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction("MeasurerDashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMeasure(OrderDetailsMeasurerViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var measure = await _context.Замеры
                .Include(z => z.ОконныеПроемы)
                .FirstOrDefaultAsync(z => z.КодЗамера == model.КодЗамера && z.КодЗамерщика == userId);

            if (measure == null) return NotFound();

            measure.ЕстьЛифт = model.ЕстьЛифт;

            if (!model.Проемы.Any(p => p.Ширина > 200 && p.Высота > 200))
            {
                var fullModel = await GetMeasureDetailsViewModel(model.КодЗаказа, userId);
                fullModel.Проемы = model.Проемы;
                ModelState.AddModelError("", "Добавьте хотя бы один проём с размерами");
                return View("MeasureDetails", fullModel);
            }

            foreach (var dto in model.Проемы.Where(p => p.Ширина > 200 || p.Высота > 200))
            {
                if (dto.КодПроема.HasValue)
                {
                    var exist = measure.ОконныеПроемы.FirstOrDefault(p => p.КодПроема == dto.КодПроема);
                    if (exist != null)
                    {
                        exist.Этаж = dto.Этаж;
                        exist.Ширина = dto.Ширина;
                        exist.Высота = dto.Высота;
                        exist.Описание = dto.Описание;
                    }
                }
                else
                {
                    measure.ОконныеПроемы.Add(new ОконныеПроемы
                    {
                        Этаж = dto.Этаж,
                        Ширина = dto.Ширина,
                        Высота = dto.Высота,
                        Описание = dto.Описание
                    });
                }
            }

            measure.КодЗаказаNavigation.КодСтатусаЗаказа = 3;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Замер сохранён";
            return RedirectToAction("MeasureDetails", new { id = model.КодЗаказа });
        }

        private async Task<OrderDetailsMeasurerViewModel> GetMeasureDetailsViewModel(int orderId, int userId)
        {
            var measure = await _context.Замеры
                .Include(z => z.ОконныеПроемы)
                .Include(z => z.КодЗаказаNavigation)
                    .ThenInclude(z => z.КодКлиентаNavigation)
                .FirstOrDefaultAsync(z => z.КодЗаказа == orderId && z.КодЗамерщика == userId);

            if (measure == null) throw new InvalidOperationException("Замер не найден");

            var model = new OrderDetailsMeasurerViewModel
            {
                КодЗамера = measure.КодЗамера,
                КодЗаказа = measure.КодЗаказа,
                ДатаЗамера = measure.ДатаЗамера,
                ЕстьЛифт = measure.ЕстьЛифт ?? false,
                КлиентФИО = $"{measure.КодЗаказаNavigation.КодКлиентаNavigation.Фамилия} {measure.КодЗаказаNavigation.КодКлиентаNavigation.Имя}",
                Телефон = measure.КодЗаказаNavigation.КодКлиентаNavigation.Телефон,
                Адрес = measure.КодЗаказаNavigation.Адрес,
                Проемы = measure.ОконныеПроемы
                    .Select(p => new WindowsOpeningViewModel
                    {
                        КодПроема = p.КодПроема,
                        Этаж = p.Этаж,
                        Ширина = p.Ширина,
                        Высота = p.Высота,
                        Описание = p.Описание
                    })
                    .ToList()
            };



            if (model.Проемы.Count == 0)
            {
                model.Проемы.Add(new WindowsOpeningViewModel { Этаж = 1 });
            }

            return model;
        }
    }
}