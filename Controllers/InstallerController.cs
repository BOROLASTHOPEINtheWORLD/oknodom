using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.Models;
using System.Security.Claims;
using System.IO;

namespace OKNODOM.Controllers
{
    [Authorize(Roles = "Монтажник")]
    public class InstallerController : Controller
    {
        private readonly OknodomDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public InstallerController(OknodomDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var hasAccess = await _context.Бригады
                .AnyAsync(b => b.КодМонтажника == userId &&
                               b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказа == id);

            if (!hasAccess)
            {
                return Forbid();
            }

            var заказ = await _context.Заказы
                .Include(z => z.КодКлиентаNavigation)
                .FirstOrDefaultAsync(z => z.КодЗаказа == id);

            var позиции = await _context.ТоварыВЗаказе
                .Where(t => t.КодЗаказа == id)
                .Include(t => t.КодТовараNavigation)
                    .ThenInclude(tov => tov.Окна)
                .Include(t => t.КодОконногоПроемаNavigation)
                .Include(t => t.Выполнения)
                .ToListAsync();

            // 👇 ЗАГРУЖАЕМ УСЛУГИ
            var услуги = await _context.УслугиВЗаказе
                .Where(u => u.КодЗаказа == id)
                .Include(u => u.КодУслугиNavigation)
                .ToListAsync();

            var всевыполненныеПозиции = позиции.All(t => t.Выполнения.Any(v => v.Фотография != null));

            if (всевыполненныеПозиции && позиции.Any())
            {
                заказ.КодСтатусаЗаказа = 6;
                _context.Update(заказ);
                await _context.SaveChangesAsync();
            }

            var appointedInstallers = await _context.Бригады
                .Where(b => b.КодВыполненияNavigation.КодТовараВЗаказеNavigation.КодЗаказа == id)
                .Select(b => b.КодМонтажникаNavigation)
                .ToListAsync();

            var viewModel = new InstallerOrderDetailsViewModel
            {
                КодЗаказа = заказ.КодЗаказа,
                Адрес = заказ.Адрес,
                ФиоКлиента = $"{заказ.КодКлиентаNavigation.Фамилия} {заказ.КодКлиентаNavigation.Имя}",
                Телефон = заказ.КодКлиентаNavigation.Телефон,
                НазначенныеМонтажники = appointedInstallers,

                // 👇 ДОБАВЛЯЕМ УСЛУГИ В МОДЕЛЬ
                Услуги = услуги.Select(u => new УслугаМонтажа
                {
                    Наименование = u.КодУслугиNavigation?.Название ?? "—",
                    Описание = u.КодУслугиNavigation?.Описание,
                    Количество = u.Количество,
                    Цена = u.ЦенаНаМоментЗаказа
                }).ToList(),

                Позиции = позиции.Select(t => new ПозицияМонтажа
                {
                    КодПозиции = t.Код,
                    Наименование = t.КодТовараNavigation?.Название ?? "—",
                    Размеры = t.КодТовараNavigation?.Окна != null
                        ? $"{t.КодТовараNavigation.Окна.Ширина}×{t.КодТовараNavigation.Окна.Высота} мм"
                        : "",
                    ПроёмЭтаж = t.КодОконногоПроемаNavigation != null
                        ? $"Этаж: {t.КодОконногоПроемаNavigation.Этаж}"
                        : null,
                    ПроёмРазмеры = t.КодОконногоПроемаNavigation != null
                        ? $"{t.КодОконногоПроемаNavigation.Ширина}×{t.КодОконногоПроемаNavigation.Высота} мм"
                        : null,
                    ПроёмОписание = t.КодОконногоПроемаNavigation?.Описание,
                    ДатаВыполнения = t.Выполнения.FirstOrDefault()?.ДатаВыполнения,
                    Выполнен = t.Выполнения.Any(v => v.Фотография != null),
                    Фотография = t.Выполнения.FirstOrDefault()?.Фотография,
                    КодВыполнения = t.Выполнения.FirstOrDefault()?.КодВыполнения ?? 0
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(int orderId, int positionId, IFormFile photo)
        {

            // Проверяем доступ
            var позиция = await _context.ТоварыВЗаказе
                .Include(t => t.Выполнения)
                .FirstOrDefaultAsync(t => t.Код == positionId && t.КодЗаказа == orderId);

            var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "uploads");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            // Обновляем выполнение
            var выполнение = позиция.Выполнения.FirstOrDefault();
            if (выполнение != null)
            {
                выполнение.Фотография = fileName;
                выполнение.ДатаВыполнения = DateTime.Now;
                _context.Update(выполнение);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Фото загружено, монтаж подтверждён";
            return RedirectToAction("OrderDetails", new { id = orderId });
        }
    }
}