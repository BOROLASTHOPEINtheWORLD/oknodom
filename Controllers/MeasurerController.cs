using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.DTOs.MeasurerDto;
using OKNODOM.Models;
using System.Security.Claims;

namespace OKNODOM.Controllers
{
    [Authorize(Roles="Замерщик")]
    public class MeasurerController : Controller
    {
        private readonly OknodomDbContext _context;
        public MeasurerController(OknodomDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MeasureDetails(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var measure = await _context.Замеры
                .Include(z=>z.ОконныеПроемы)
                .Include(z=>z.КодЗаказаNavigation)
                    .ThenInclude(z=>z.КодКлиентаNavigation)
                .FirstOrDefaultAsync(z=> z.КодЗаказа == id && z.КодЗамерщика == userId);
            if(measure == null) return RedirectToAction("Login", "Account");

            var model = new OrderDetailsMeasurerViewModel
            {
                КодЗамера = measure.КодЗамера,
                КодЗаказа = measure.КодЗаказа,
                ДатаЗамера = measure.ДатаЗамера,
                ЕстьЛифт = measure.ЕстьЛифт ?? false,
                КлиентФИО = measure.КодЗаказаNavigation.КодКлиентаNavigation.Фамилия + " "
                    + measure.КодЗаказаNavigation.КодКлиентаNavigation.Имя + " "
                    + measure.КодЗаказаNavigation.КодКлиентаNavigation.Отчество,
                Адрес = measure.КодЗаказаNavigation.Адрес,
                Проемы = measure.ОконныеПроемы.Select(p => new WindowsOpeningViewModel
                {
                    КодПроема = p.КодПроема,
                    Этаж = p.Этаж,
                    Ширина = p.Ширина,
                    Высота = p.Высота,
                    Описание = p.Описание
                }).ToList()
            };

            if (model.Проемы.Count == 0)
                model.Проемы.Add(new WindowsOpeningViewModel { Этаж = 1 });

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMeasure(OrderDetailsMeasurerViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var measure = await _context.Замеры
                .Include(z=>z.ОконныеПроемы)
                .FirstOrDefaultAsync(z=>z.КодЗамера == model.КодЗамера && z.КодЗамерщика == userId);
            if (measure == null) return NotFound();

            measure.ДатаЗамера = model.ДатаЗамера;
            measure.ЕстьЛифт = model.ЕстьЛифт;

            foreach(var dto in model.Проемы.Where(p=>p.Ширина > 0 || p.Высота > 0))
            {
                if(dto.КодПроема.HasValue)
                {
                    var exist = measure.ОконныеПроемы.FirstOrDefault(p=>p.КодПроема == dto.КодПроема.Value);
                    if (exist!=null)
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
            await _context.SaveChangesAsync();
            TempData["SuccesMessage"] = "Замер сохранён";
            return RedirectToAction("MeasureDetails", new { id = model.КодЗаказа });
        }
    }
}
