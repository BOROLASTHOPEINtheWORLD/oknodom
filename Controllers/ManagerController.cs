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
                 .Include(z => z.ТоварыВЗаказе)
                     .ThenInclude(t => t.КодТовараNavigation)
                         .ThenInclude(t => t.Окна) 
                 .Include(z => z.ТоварыВЗаказе)
                     .ThenInclude(t => t.КодТовараNavigation)
                         .ThenInclude(t => t.Комплектующие)
                 .Include(z => z.УслугиВЗаказе)
                     .ThenInclude(u => u.КодУслугиNavigation)
                 .Include(z => z.Замеры)
                     .ThenInclude(m => m.КодЗамерщикаNavigation)
                 .Include(z => z.Замеры)
                     .ThenInclude(m => m.ОконныеПроемы) 
                 .FirstOrDefaultAsync(z => z.КодЗаказа == id);

            if (order == null)
            {
                return NotFound();
            }

            var measurers = await _context.Пользователи
                .Where(u => u.КодРоли == 3)
                .OrderBy(u => u.Фамилия)
                .ToListAsync();

            var allProducts = await _context.Товары
                .Include(t => t.Окна)
                .Include(t => t.Комплектующие)
                .Where(t => t.Активный)
                .ToListAsync();

            var allServices = await _context.Услуги
                .Where(u => u.Активна)
                .ToListAsync();
            var viewModel = new OrderDetailsManagerViewModel
            {
                Заказ = order,
                Клиент = order.КодКлиентаNavigation, 
                Замерщики = measurers,
                ТекущийЗамер = order.Замеры?.FirstOrDefault(),
                ТекущиеТовары = order.ТоварыВЗаказе.ToList(),
                ТекущиеУслуги = order.УслугиВЗаказе.ToList(),
                Товары = allProducts, 
                Услуги = allServices                  
            };

            return View(viewModel);
        }
    
        public async Task<IActionResult> OrderConfigure(int id, string filterType = "all")
        {
            // Загружаем заказ + связанные данные
            var order = await _context.Заказы
                .Include(o => o.ТоварыВЗаказе)
                    .ThenInclude(t => t.КодТовараNavigation)
                        .ThenInclude(p => p.Окна)
                .Include(o => o.УслугиВЗаказе)
                .Include(o => o.Замеры)
                    .ThenInclude(z => z.ОконныеПроемы)
                .FirstOrDefaultAsync(o => o.КодЗаказа == id);

 

            // === Товары ===
            var products = await _context.Товары
                .Include(t => t.Окна)
                    .ThenInclude(o => o.КодПрофиляNavigation)  
                .Include(t => t.Окна)
                    .ThenInclude(o => o.КодСтеклопакетаNavigation)  
                .Include(t => t.Окна)
                    .ThenInclude(o => o.Створки)      
                        .ThenInclude(s => s.КодТипаСтворкиNavigation)
                .Include(t => t.Комплектующие)
                .Where(t => t.Активный)
                .ToListAsync();

            var productDtos = products.Select(p => new ProductsForConfiguration
            {
                КодТовара = p.КодТовара,
                Название = p.Название,
                Цена = p.Цена,
                Цвет = p.Цвет,
                ЯвляетсяОкном = p.Окна != null,
                Размеры = GetDimensions(p),
                ГарантияМесяцев = p.Окна?.БазоваяГарантияМесяцев ?? 12,
                ДеталиОкна = GetWindowInfo(p.Окна),
                Фото = p.Фото
            }).ToList();



            // === Услуги ===
            var serviceDtos = await _context.Услуги
                .Where(u => u.Активна)
                .OrderBy(u => u.Название)
                .Select(u => new ServicesForConfiguration
                {
                    КодУслуги = u.КодУслуги,
                    Название = u.Название,
                    Цена = u.БазоваяСтоимость,
                    Описание = u.Описание
                })
                .ToListAsync();

            // === Текущая конфигурация ===
            var currentProducts = order.ТоварыВЗаказе
                .ToDictionary(t => t.КодТовара, t => t.Количество);

            var currentServices = order.УслугиВЗаказе
                .ToDictionary(u => u.КодУслуги, u => u.Количество);

            // === Проёмы (только если есть замер) ===
            var measurement = order.Замеры?.OrderByDescending(z => z.ДатаЗамера).FirstOrDefault();
            var proems = measurement?.ОконныеПроемы?.Select(p => new WindowOpeningForSelection
            {
                КодПроема = p.КодПроема,
                Этаж = p.Этаж,
                Ширина = p.Ширина,
                Высота = p.Высота,
                Описание = p.Описание ?? ""
            }).ToList() ?? new List<WindowOpeningForSelection>();

            // === Текущая привязка окон к проёмам (только для окон!) ===
            var bind = order.ТоварыВЗаказе
                .Where(t => t.КодТовараNavigation?.Окна != null) // только окна
                .ToDictionary(
                    t => t.КодТовара,
                    t => (int?)t.КодОконногоПроема 
                );

            var viewModel = new OrderConfigurationViewModel
            {
                КодЗаказа = id,
                ДоступныеТовары = productDtos,
                ДоступныеУслуги = serviceDtos,
                ТекущиеТовары = currentProducts,
                ТекущиеУслуги = currentServices,
                ДоступныеПроемы = proems,
                ПривязкаОконКПроемам = bind,
                ВыбранныйФильтр = filterType
            };

            return View(viewModel); 
        }

        private string GetDimensions(Товары product)
        {
            if (product.Окна != null)
            {
                return $"{product.Окна.Ширина}×{product.Окна.Высота} мм";
            }
            else if (product.Комплектующие != null)
            {
                return $"{product.Комплектующие.ШиринаМм}×{product.Комплектующие.ДлинаМм} мм";
            }
            return "Размер не указан";
        }
        private WindowDetails? GetWindowInfo(Окна? окно)
        {
            if (окно == null) return null;

            return new WindowDetails
            {
                КоличествоСтворок = окно.КоличествоСтворок,
                Стандартное = окно.Стандартное,

                // Профиль
                НазваниеПрофиля = окно.КодПрофиляNavigation?.Название,
                ШиринаПрофиля = окно.КодПрофиляNavigation?.Ширина,
                КоличествоКамер = окно.КодПрофиляNavigation?.КоличествоКамер,
                СопротивлениеТеплопередаче = окно.КодПрофиляNavigation?.СопротивлениеТеплопередаче,
                Звукоизоляция = окно.КодПрофиляNavigation?.Звукоизоляция,
                ТолщинаСтеклопакета = окно.КодПрофиляNavigation?.ТолщинаСтеклопакета,

                НазваниеСтеклопакета = окно.КодСтеклопакетаNavigation?.Название,

                Створки = окно.Створки?.Select(s => new SashInfo
                {
                    НомерСтворки = s.НомерСтворки,
                    ТипСтворки = s.КодТипаСтворкиNavigation?.Название ?? "Не указан",
                    Описание = s.КодТипаСтворкиNavigation?.Описание
                }).OrderBy(s => s.НомерСтворки).ToList()
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderConfigure(int orderId, Dictionary<int, int> products, Dictionary<int, int> services, Dictionary<int, int?> windowToOpening)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Заказы
                    .Include(o => o.ТоварыВЗаказе)
                    .Include(o => o.УслугиВЗаказе)
                    .FirstOrDefaultAsync(o => o.КодЗаказа == orderId);

                // === ВАЛИДАЦИЯ РАЗМЕРОВ ===
                if (windowToOpening != null && windowToOpening.Any())
                {
                    // Получаем все окна из products
                    var windowProductIds = products?
                        .Where(p => p.Value > 0)
                        .Select(p => p.Key)
                        .ToList() ?? new List<int>();

                    if (windowProductIds.Any())
                    {
                        // Загружаем информацию об окнах
                        var windowsInfo = await _context.Товары
                            .Include(t => t.Окна)
                            .Where(t => windowProductIds.Contains(t.КодТовара) && t.Окна != null)
                            .ToDictionaryAsync(t => t.КодТовара);

                        // Загружаем информацию о проемах этого заказа
                        var openings = await _context.Замеры
                            .Where(z => z.КодЗаказа == orderId)
                            .SelectMany(z => z.ОконныеПроемы)
                            .ToDictionaryAsync(o => o.КодПроема);

                        // Проверяем каждое окно
                        foreach (var kvp in windowToOpening)
                        {
                            if (kvp.Value.HasValue) // если окно привязано к проему
                            {
                                var productId = kvp.Key;
                                var openingId = kvp.Value.Value;

                                if (windowsInfo.TryGetValue(productId, out var product) &&
                                    product.Окна != null &&
                                    openings.TryGetValue(openingId, out var opening))
                                {
                                    // Простая проверка: проем должен быть больше окна
                                    if (product.Окна.Ширина >= opening.Ширина ||
                                        product.Окна.Высота >= opening.Высота)
                                    {
                                        TempData["ErrorMessage"] =
                                            $"Окно '{product.Название}' ({product.Окна.Ширина}×{product.Окна.Высота} мм) " +
                                            $"не подходит для проема {openingId} ({opening.Ширина}×{opening.Высота} мм). " +
                                            "Проем должен быть больше окна!";
                                        return RedirectToAction("OrderConfigure", new { id = orderId });
                                    }

                                    // Проверка минимального зазора (20 мм с каждой стороны)
                                    int minGap = 40; // 20 мм с двух сторон = 40 мм

                                    int widthGap = opening.Ширина - product.Окна.Ширина;
                                    int heightGap = opening.Высота - product.Окна.Высота;

                                    if (widthGap < minGap || heightGap < minGap)
                                    {
                                        TempData["WarningMessage"] =
                                            $"Внимание! Маленький зазор у окна '{product.Название}': " +
                                            $"ширина {widthGap} мм, высота {heightGap} мм. " +
                                            $"Рекомендуется минимум {minGap} мм.";
                                    }
                                }
                            }
                        } 
                    } 
                } 
             

                
                bool hasProducts = products?.Any(p => p.Value > 0) == true;
                bool hasServices = services?.Any(s => s.Value > 0) == true;

                if (!hasProducts && !hasServices)
                {
                    TempData["ErrorMessage"] = "Выберите хотя бы один товар или услугу";
                    return RedirectToAction("OrderConfigure", new { id = orderId });
                }

                _context.ТоварыВЗаказе.RemoveRange(order.ТоварыВЗаказе);
                _context.УслугиВЗаказе.RemoveRange(order.УслугиВЗаказе);

                decimal totalAmount = 0;

                if (hasProducts)
                {
                    var productsWithQuantity = products.Where(p => p.Value > 0).ToDictionary();
                    var productIds = productsWithQuantity.Keys.ToArray();

                    var productsInDb = await _context.Товары
                        .Include(t => t.Окна)
                        .Where(t => productIds.Contains(t.КодТовара) && t.Активный)
                        .ToDictionaryAsync(t => t.КодТовара);

                    foreach (var kvp in productsWithQuantity)
                    {
                        if (productsInDb.TryGetValue(kvp.Key, out var product))
                        {
                            decimal price = product.Цена;
                            int quantity = kvp.Value;
                            decimal itemTotal = price * quantity;
                            totalAmount += itemTotal;

                            // Warranty
                            int warrantyMonths = 12;
                            if (product.Окна != null && product.Окна.БазоваяГарантияМесяцев > 0)
                            {
                                warrantyMonths = product.Окна.БазоваяГарантияМесяцев;
                            }

                            DateOnly warrantyUntil = DateOnly.FromDateTime(
                                DateTime.Now.AddMonths(warrantyMonths));

                            var товарВЗаказе = new ТоварыВЗаказе
                            {
                                КодЗаказа = orderId,
                                КодТовара = kvp.Key,
                                Количество = quantity,
                                ЦенаНаМоментЗаказа = price,
                                ГарантияМесяцев = warrantyMonths,
                                ГарантияДо = warrantyUntil
                            };

                            // Сохраняем привязку к проему для окон
                            if (windowToOpening != null &&
                                windowToOpening.TryGetValue(kvp.Key, out var openingId) &&
                                openingId.HasValue)
                            {
                                товарВЗаказе.КодОконногоПроема = openingId.Value;
                            }

                            _context.ТоварыВЗаказе.Add(товарВЗаказе);
                        }
                    }
                }

                if (hasServices)
                {
                    var servicesWithQuantity = services.Where(s => s.Value > 0).ToDictionary();
                    var serviceIds = servicesWithQuantity.Keys.ToArray();

                    var servicesInDb = await _context.Услуги
                        .Where(u => serviceIds.Contains(u.КодУслуги) && u.Активна)
                        .ToDictionaryAsync(u => u.КодУслуги);

                    foreach (var kvp in servicesWithQuantity)
                    {
                        if (servicesInDb.TryGetValue(kvp.Key, out var service))
                        {
                            decimal price = service.БазоваяСтоимость;
                            int quantity = kvp.Value;
                            decimal itemTotal = price * quantity;
                            totalAmount += itemTotal;

                            _context.УслугиВЗаказе.Add(new УслугиВЗаказе
                            {
                                КодЗаказа = orderId,
                                КодУслуги = kvp.Key,
                                Количество = quantity,
                                ЦенаНаМоментЗаказа = price
                            });
                        }
                    }
                }

                // Update status and total
                order.КодСтатусаЗаказа = 8;
                order.Стоимость = totalAmount;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"Конфигурация сохранена! Общая сумма: {totalAmount:N0}₽";
                return RedirectToAction("OrderDetails", new { id = orderId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Ошибка при сохранении: {ex.Message}";
                return RedirectToAction("OrderConfigure", new { id = orderId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMeasurer(AssignMeasureViewModel model)
        {
            var order = await _context.Заказы
                .FirstOrDefaultAsync(z => z.КодЗаказа == model.КодЗаказа);

            if (order == null || order.КодСтатусаЗаказа != 1)
            {
                TempData["ErrorMessage"] = "Невозможно назначить замерщика";
                return RedirectToAction("ManagerDashboard", "Account");
            }

            var newMeasurement = new Замеры
            {
                КодЗаказа = model.КодЗаказа,
                КодЗамерщика = model.КодЗамерщика,
                ДатаЗамера = model.ДатаЗамера
            };

            _context.Замеры.Add(newMeasurement);
            order.КодСтатусаЗаказа = 2;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Замерщик успешно назначен на заказ №{model.КодЗаказа}";
            return RedirectToAction("OrderDetails", new { id = model.КодЗаказа });
        }
    }
}