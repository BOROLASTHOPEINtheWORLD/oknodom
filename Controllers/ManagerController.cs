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
            var всеПроёмыПоТоварам = order.ТоварыВЗаказе
                .Where(t => t.КодТовараNavigation?.Окна != null && t.КодОконногоПроема.HasValue)
                .GroupBy(t => t.КодТовара)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(t => t.КодОконногоПроема.Value).ToList());

            ViewData["ВсеПроёмыПоТоварам"] = всеПроёмыПоТоварам;

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

            if (order == null)
            {
                TempData["ErrorMessage"] = "Заказ не найден";
                return RedirectToAction("Index");
            }

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
                .GroupBy(t => t.КодТовара)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Количество));

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

            // === Текущая привязка окон к проёмам (ПЕРВЫЙ проём для каждого товара) ===
            var bind = order.ТоварыВЗаказе
                .Where(t => t.КодТовараNavigation?.Окна != null && t.КодОконногоПроема.HasValue)
                .GroupBy(t => t.КодТовара)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault()?.КодОконногоПроема ?? 0
                );

            // === ВСЕ проёмы для каждого товара (новое свойство) ===
            var всеПроёмыПоТоварам = new Dictionary<int, List<int>>();

            var окнаВЗаказе = order.ТоварыВЗаказе
                .Where(t => t.КодТовараNavigation?.Окна != null && t.КодОконногоПроема.HasValue)
                .OrderBy(t => t.КодТовара);

            foreach (var товарВЗаказе in окнаВЗаказе)
            {
                var productId = товарВЗаказе.КодТовара;

                if (!всеПроёмыПоТоварам.ContainsKey(productId))
                {
                    всеПроёмыПоТоварам[productId] = new List<int>();
                }

                // Добавляем проём столько раз, сколько повторяется этот товар
                // (если Количество > 1, значит одинаковые окна в разных проёмах)
                for (int i = 0; i < товарВЗаказе.Количество; i++)
                {
                    всеПроёмыПоТоварам[productId].Add(товарВЗаказе.КодОконногоПроема.Value);
                }
            }

            // Также нужно учесть, что товары с Количество > 1 могут быть сохранены как одна запись
            // В этом случае просто повторяем проём нужное количество раз
            // (В вашем POST-методе они сохраняются отдельными записями, так что это не требуется)

            var viewModel = new OrderConfigurationViewModel
            {
                КодЗаказа = id,
                ДоступныеТовары = productDtos,
                ДоступныеУслуги = serviceDtos,
                ТекущиеТовары = currentProducts,
                ТекущиеУслуги = currentServices,
                ДоступныеПроемы = proems,
                ПривязкаОконКПроемам = bind,          // Только первый проём для совместимости
                ВсеПроёмыПоТоварам = всеПроёмыПоТоварам, // Все проёмы для восстановления
                ВыбранныйФильтр = filterType
            };

            // Передаем данные в ViewData для JavaScript
            ViewData["ВсеПроёмыПоТоварам"] = всеПроёмыПоТоварам;

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
        public async Task<IActionResult> OrderConfigure(OrderConfigurationPostModel model)
        {
            int orderId = model.OrderId;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Заказы
                    .Include(o => o.ТоварыВЗаказе)
                    .Include(o => o.УслугиВЗаказе)
                    .FirstOrDefaultAsync(o => o.КодЗаказа == orderId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Заказ не найден";
                    return RedirectToAction("OrderConfigure", new { id = orderId });
                }

                // === ПРЕОБРАЗОВАНИЕ ДАННЫХ ИЗ ФОРМЫ ===
                // Получаем информацию о товарах из БД
                var всеТовары = await _context.Товары
                    .Include(t => t.Окна)
                    .Where(t => model.Products.Keys.Contains(t.КодТовара))
                    .ToDictionaryAsync(t => t.КодТовара);

                // Создаем списки окон и комплектующих
                var windowItems = new List<WindowItemDto>();
                var accessories = new Dictionary<int, int>();
                var services = model.Services;

                foreach (var kvp in model.Products.Where(p => p.Value > 0))
                {
                    if (всеТовары.TryGetValue(kvp.Key, out var товар))
                    {
                        if (товар.Окна != null)
                        {
                            // Это окно - обрабатываем основные и дополнительные окна

                            // 1. Основное окно (ключ: "2")
                            string основнойКлюч = kvp.Key.ToString();
                            if (model.WindowToOpening.TryGetValue(основнойКлюч, out var основнойПроемId) &&
                                основнойПроемId > 0 && kvp.Value >= 1)
                            {
                                windowItems.Add(new WindowItemDto
                                {
                                    КодТовара = kvp.Key,
                                    Количество = 1,
                                    КодПроема = основнойПроемId
                                });
                            }

                            // 2. Дополнительные окна (ключи: "2_2", "2_3", и т.д.)
                            for (int i = 2; i <= kvp.Value; i++)
                            {
                                string дополнительныйКлюч = $"{kvp.Key}_{i}";
                                if (model.WindowToOpening.TryGetValue(дополнительныйКлюч, out var допПроемId) &&
                                    допПроемId > 0)
                                {
                                    windowItems.Add(new WindowItemDto
                                    {
                                        КодТовара = kvp.Key,
                                        Количество = 1,
                                        КодПроема = допПроемId
                                    });
                                }
                            }
                        }
                        else
                        {
                            // Это комплектующее
                            accessories[kvp.Key] = kvp.Value;
                        }
                    }
                }

                // === ВАЛИДАЦИЯ РАЗМЕРОВ ОКОН ===
                if (windowItems.Any())
                {
                    var windowProductIds = windowItems
                        .Select(w => w.КодТовара)
                        .Distinct()
                        .ToList();

                    var windowsInfo = await _context.Товары
                        .Include(t => t.Окна)
                        .Where(t => windowProductIds.Contains(t.КодТовара) && t.Окна != null)
                        .ToDictionaryAsync(t => t.КодТовара);

                    var openings = await _context.Замеры
                        .Where(z => z.КодЗаказа == orderId)
                        .SelectMany(z => z.ОконныеПроемы)
                        .ToDictionaryAsync(o => o.КодПроема);

                    foreach (var item in windowItems)
                    {
                        var productId = item.КодТовара;
                        var openingId = item.КодПроема;

                        if (windowsInfo.TryGetValue(productId, out var product) &&
                            product.Окна != null &&
                            openings.TryGetValue(openingId, out var opening))
                        {
                            // Проверка: окно должно быть меньше проёма
                            if (product.Окна.Ширина >= opening.Ширина ||
                                product.Окна.Высота >= opening.Высота)
                            {
                                TempData["ErrorMessage"] =
                                    $"Окно '{product.Название}' ({product.Окна.Ширина}×{product.Окна.Высота} мм) " +
                                    $"не подходит для проема ({opening.Ширина}×{opening.Высота} мм). " +
                                    "Проем должен быть больше окна!";
                                return RedirectToAction("OrderConfigure", new { id = orderId });
                            }

                            // Проверка зазора
                            int minGap = 40;
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
                        else if (product?.Окна != null)
                        {
                            TempData["ErrorMessage"] = $"Проём {openingId} не найден для окна '{product.Название}'";
                            return RedirectToAction("OrderConfigure", new { id = orderId });
                        }
                    }
                }

                bool hasWindows = windowItems.Any();
                bool hasAccessories = accessories.Any(a => a.Value > 0);
                bool hasServices = services.Any(s => s.Value > 0);

                if (!hasWindows && !hasAccessories && !hasServices)
                {
                    TempData["ErrorMessage"] = "Выберите хотя бы один товар или услугу";
                    return RedirectToAction("OrderConfigure", new { id = orderId });
                }

                // Удаляем старые записи
                _context.ТоварыВЗаказе.RemoveRange(order.ТоварыВЗаказе);
                _context.УслугиВЗаказе.RemoveRange(order.УслугиВЗаказе);

                decimal totalAmount = 0;

                // Сохраняем окна
                if (hasWindows)
                {
                    var windowProductIds = windowItems.Select(w => w.КодТовара).Distinct().ToArray();
                    var windowsInDb = await _context.Товары
                        .Include(t => t.Окна)
                        .Where(t => windowProductIds.Contains(t.КодТовара) && t.Активный)
                        .ToDictionaryAsync(t => t.КодТовара);

                    // Группируем окна по товару и проёму
                    var сгруппированныеОкна = windowItems
                        .GroupBy(w => new { w.КодТовара, w.КодПроема })
                        .Select(g => new
                        {
                            g.Key.КодТовара,
                            g.Key.КодПроема,
                            Количество = g.Count()
                        });

                    foreach (var группа in сгруппированныеОкна)
                    {
                        if (windowsInDb.TryGetValue(группа.КодТовара, out var product))
                        {
                            decimal price = product.Цена;
                            int quantity = группа.Количество;
                            totalAmount += price * quantity;

                            int warrantyMonths = product.Окна?.БазоваяГарантияМесяцев ?? 12;
                            DateOnly warrantyUntil = DateOnly.FromDateTime(DateTime.Now.AddMonths(warrantyMonths));

                            _context.ТоварыВЗаказе.Add(new ТоварыВЗаказе
                            {
                                КодЗаказа = orderId,
                                КодТовара = группа.КодТовара,
                                Количество = quantity,
                                ЦенаНаМоментЗаказа = price,
                                ГарантияМесяцев = warrantyMonths,
                                ГарантияДо = warrantyUntil,
                                КодОконногоПроема = группа.КодПроема
                            });
                        }
                    }
                }

                // Сохраняем услуги
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

                // Сохраняем комплектующие
                if (hasAccessories)
                {
                    var accessoryIds = accessories.Keys.ToArray();
                    var accessoriesInDb = await _context.Товары
                        .Where(t => accessoryIds.Contains(t.КодТовара) && t.Активный && t.Окна == null)
                        .ToDictionaryAsync(t => t.КодТовара);

                    foreach (var kvp in accessories.Where(a => a.Value > 0))
                    {
                        if (accessoriesInDb.TryGetValue(kvp.Key, out var accessory))
                        {
                            decimal price = accessory.Цена;
                            int quantity = kvp.Value;
                            totalAmount += price * quantity;

                            _context.ТоварыВЗаказе.Add(new ТоварыВЗаказе
                            {
                                КодЗаказа = orderId,
                                КодТовара = kvp.Key,
                                Количество = quantity,
                                ЦенаНаМоментЗаказа = price,
                                ГарантияМесяцев = 0,
                                ГарантияДо = null
                            });
                        }
                    }
                }

                // Обновляем статус и сумму заказа
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