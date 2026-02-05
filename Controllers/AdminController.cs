using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.Models;
using OKNODOM.Services;
using System.Security.Claims;


namespace OKNODOM.Controllers;

[Authorize(Roles = "Админ")]
public class AdminController : Controller
{
    private readonly OknodomDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly OrderDetailsService _orderDetailsService;
    public AdminController(OknodomDbContext context, IWebHostEnvironment environment, OrderDetailsService orderDetailsService)
    {
        _context = context;
        _environment = environment;
        _orderDetailsService = orderDetailsService;
    }

    public async Task<IActionResult> Users(string search = "", int roleFilter = 0)
    {
        ViewBag.ActivePage = "Users";
        ViewBag.Search = search;
        ViewBag.RoleFilter = roleFilter;

        IQueryable<Пользователи> query = _context.Пользователи
            .Include(u => u.КодРолиNavigation);

        // Поиск по ФИО
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(u =>
                u.Фамилия.ToLower().Contains(normalized) ||
                u.Имя.ToLower().Contains(normalized) ||
                u.Отчество.ToLower().Contains(normalized));
        }

        // Фильтр по роли
        if (roleFilter > 0)
        {
            query = query.Where(u => u.КодРоли == roleFilter);
        }

        var users = await query
            .OrderBy(u => u.Фамилия)
            .ThenBy(u => u.Имя)
            .ToListAsync();

        var roles = await _context.Роли.ToListAsync();
        ViewBag.Roles = roles;

        return View(users);
    }
    // === ФОРМА СОЗДАНИЯ ===
    public async Task<IActionResult> CreateUser()
    {
        ViewBag.Roles = await _context.Роли.ToListAsync();
        ViewData["ActivePage"] = "Users";
        ViewBag.ClientRoleId = 5; // Передаем в View
        return View();
    }

    // === СОХРАНЕНИЕ НОВОГО ПОЛЬЗОВАТЕЛЯ ===
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(Пользователи user)
    {
        // УДАЛЯЕМ ОШИБКИ ДЛЯ НАВИГАЦИОННЫХ СВОЙСТВ
        ModelState.Remove("КодРолиNavigation");

        if (string.IsNullOrWhiteSpace(user.Пароль))
        {
            ModelState.AddModelError("Пароль", "Пароль обязателен");
        }

        // Простая проверка - если это клиент
        if (user.КодРоли == 5)
        {
            user.Паспорт = null; // Очищаем паспорт для клиента
        }
        // Если НЕ клиент и паспорт пустой - ошибка
        else if (string.IsNullOrWhiteSpace(user.Паспорт))
        {
            ModelState.AddModelError("Паспорт", "Паспорт обязателен для этой роли");
        }

        if (ModelState.IsValid)
        {
            _context.Пользователи.Add(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Пользователь успешно добавлен";
            return RedirectToAction(nameof(Users));
        }

        ViewBag.Roles = await _context.Роли.ToListAsync();
        ViewBag.ClientRoleId = 5;
        return View(user);
    }

    // === ФОРМА РЕДАКТИРОВАНИЯ ===
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _context.Пользователи.FindAsync(id);
        if (user == null) return NotFound();

        ViewData["ActivePage"] = "Users";
        ViewBag.Roles = await _context.Роли.ToListAsync();
        ViewBag.ClientRoleId = 5;
        return View(user);
    }

    // === СОХРАНЕНИЕ ИЗМЕНЕНИЙ ===
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(Пользователи user)
    {
        ModelState.Remove("Пароль");
        ModelState.Remove("КодРолиNavigation");

        // Получаем существующего пользователя
        var existingUser = await _context.Пользователи
            .FirstOrDefaultAsync(u => u.КодПользователя == user.КодПользователя);

        if (existingUser == null)
        {
            return NotFound();
        }

        // Если пароль не указан в форме - используем старый
        if (string.IsNullOrWhiteSpace(user.Пароль))
        {
            user.Пароль = existingUser.Пароль;
        }

        // Проверка паспорта
        if (user.КодРоли == 5)
        {
            user.Паспорт = null;
        }
        else if (string.IsNullOrWhiteSpace(user.Паспорт))
        {
            ModelState.AddModelError("Паспорт", "Паспорт обязателен для этой роли");
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Обновляем пользователя
                existingUser.Фамилия = user.Фамилия;
                existingUser.Имя = user.Имя;
                existingUser.Отчество = user.Отчество;
                existingUser.Телефон = user.Телефон;
                existingUser.КодРоли = user.КодРоли;
                existingUser.Активный = user.Активный;
                existingUser.Паспорт = user.Паспорт;

                // Обновляем пароль только если он был изменен
                if (!string.IsNullOrWhiteSpace(user.Пароль) && user.Пароль != existingUser.Пароль)
                {
                    existingUser.Пароль = user.Пароль;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Пользователь успешно обновлён";
                return RedirectToAction(nameof(Users));
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UX_Пользователи_Логин") == true)
            {
                ModelState.AddModelError("Логин", "Пользователь с таким логином уже существует");
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UX_Пользователи_Телефон") == true)
            {
                ModelState.AddModelError("Телефон", "Пользователь с таким телефоном уже существует");
            }
        }

        ViewBag.Roles = await _context.Роли.ToListAsync();
        ViewBag.ClientRoleId = 5;
        return View(user);
    }
    // === ДЕАКТИВАЦИЯ ПОЛЬЗОВАТЕЛЯ ===
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var user = await _context.Пользователи.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Если пользователь уже не активен
        if (!user.Активный)
        {
            TempData["WarningMessage"] = "Пользователь уже неактивен";
            return RedirectToAction(nameof(Users));
        }

        try
        {
            user.Активный = false;
            _context.Update(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Пользователь успешно деактивирован";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка при деактивации: {ex.Message}";
        }

        return RedirectToAction(nameof(Users));
    }

    // === АКТИВАЦИЯ ПОЛЬЗОВАТЕЛЯ ===
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateUser(int id)
    {
        var user = await _context.Пользователи.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Если пользователь уже активен
        if (user.Активный)
        {
            TempData["WarningMessage"] = "Пользователь уже активен";
            return RedirectToAction(nameof(Users));
        }

        try
        {
            user.Активный = true;
            _context.Update(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Пользователь успешно активирован";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка при активации: {ex.Message}";
        }

        return RedirectToAction(nameof(Users));
    }

    // === СПИСОК ТОВАРОВ ===
    public async Task<IActionResult> Products(string search = "", int typeFilter = 0)
    {
        var query = _context.Товары.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim().ToLower();
            query = query.Where(t => t.Название.ToLower().Contains(normalized));
        }

        if (typeFilter > 0)
        {
            query = query.Where(t => t.КодТипаТовара == typeFilter);
        }

        var products = await query
            .Include(t => t.КодТипаТовараNavigation)
            .OrderBy(t => t.Название)
            .ToListAsync();

        ViewBag.ActivePage = "Products";
        ViewBag.Search = search;
        ViewBag.TypeFilter = typeFilter;
        ViewBag.Types = await _context.ТипыТоваров.ToListAsync();

        return View(products);
    }

    // === ФОРМА РЕДАКТИРОВАНИЯ / СОЗДАНИЯ ===
    public async Task<IActionResult> EditProduct(int? id = null, int? type = null)
    {
        ViewBag.ActivePage = "Products";
        var model = new ProductEditModel();

        if (id.HasValue)
        {
            // Редактирование
            var товар = await _context.Товары
                .Include(t => t.Окна)
                .ThenInclude(o => o.Створки)
                .Include(t => t.Комплектующие)
                .FirstOrDefaultAsync(t => t.КодТовара == id.Value);

            if (товар == null) return NotFound();

            model.КодТовара = товар.КодТовара;
            model.КодТипаТовара = товар.КодТипаТовара;
            model.Название = товар.Название;
            model.Цена = товар.Цена;
            model.Цвет = товар.Цвет;
            model.Фото = товар.Фото;
            model.Активный = товар.Активный;

            if (товар.Окна != null)
            {
                model.КодПрофиля = товар.Окна.КодПрофиля;
                model.КодСтеклопакета = товар.Окна.КодСтеклопакета;
                model.Ширина = товар.Окна.Ширина;
                model.Высота = товар.Окна.Высота;
                model.Стандартное = товар.Окна.Стандартное;
                model.КоличествоСтворок = товар.Окна.КоличествоСтворок;
                model.БазоваяГарантияМесяцев = товар.Окна.БазоваяГарантияМесяцев;

                model.Створки = товар.Окна.Створки.Select(s => new ProductEditModel.WindowSash
                {
                    КодСтворки = s.КодСтворки,
                    КодТипаСтворки = s.КодТипаСтворки,
                    НомерСтворки = s.НомерСтворки
                }).ToList();
            }
            else if (товар.Комплектующие != null)
            {
                model.КодТипаКомплектующего = товар.Комплектующие.КодТипаКомплектующего;
                model.КодМатериала = товар.Комплектующие.КодМатериала;
                model.ДлинаМм = товар.Комплектующие.ДлинаМм;
                model.ШиринаМм = товар.Комплектующие.ШиринаМм;
                model.ВесКг = товар.Комплектующие.ВесКг;
            }
        }
        else if (type.HasValue)
        {
            // Создание
            model.КодТипаТовара = type.Value;
            if (type.Value == 1) // Окно
            {
                model.КоличествоСтворок = 1;
                model.Створки.Add(new ProductEditModel.WindowSash { НомерСтворки = 1 });
            }
        }
        else
        {
            return BadRequest("Не указан ни ID, ни тип");
        }

        // Загружаем справочники
        model.Профили = await _context.Профили.ToListAsync();
        model.Стеклопакеты = await _context.Стеклопакеты.ToListAsync();
        model.ТипыКомплектующих = await _context.ТипыКомплектующих.ToListAsync();
        model.Материалы = await _context.Материалы.ToListAsync();
        model.ТипыСтворок = await _context.ТипыСтворок.ToListAsync();
        model.ТипыТоваров = await _context.ТипыТоваров.ToListAsync();
        ViewBag.ActivePage = "Products";
        return View(model);
    }

    // === СОХРАНЕНИЕ ===
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProduct(ProductEditModel model, IFormFile фотоФайл)
    {
        if (!ModelState.IsValid)
        {
            model.Профили = await _context.Профили.ToListAsync();
            model.Стеклопакеты = await _context.Стеклопакеты.ToListAsync();
            model.ТипыКомплектующих = await _context.ТипыКомплектующих.ToListAsync();
            model.Материалы = await _context.Материалы.ToListAsync();
            model.ТипыСтворок = await _context.ТипыСтворок.ToListAsync();
            model.ТипыТоваров = await _context.ТипыТоваров.ToListAsync();
            return View("EditProduct", model);
        }

        if (model.КодТовара.HasValue)
        {
            // Редактирование
            var товар = await _context.Товары
                .Include(t => t.Окна)
                .ThenInclude(o => o.Створки)
                .Include(t => t.Комплектующие)
                .FirstOrDefaultAsync(t => t.КодТовара == model.КодТовара.Value);

            if (товар == null) return NotFound();
            var староеФото = товар.Фото;
            // Обновляем фото если загружено новое
            if (фотоФайл != null && фотоФайл.Length > 0)
            {
                // Удаляем старое фото если оно есть
                if (!string.IsNullOrEmpty(товар.Фото))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, "images", "products", товар.Фото);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Сохраняем новое фото
                var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadFolder);


                var fileName = Path.GetFileName(фотоФайл.FileName);
                var filePath = Path.Combine(uploadFolder, fileName);


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await фотоФайл.CopyToAsync(stream);
                }

                товар.Фото = fileName;
            }

            товар.Название = model.Название;
            товар.Цена = model.Цена;
            товар.Цвет = model.Цвет;
            товар.Активный = model.Активный;

            if (товар.Окна != null && model.КодТипаТовара == 1)
            {
                товар.Окна.КодПрофиля = model.КодПрофиля.Value;
                товар.Окна.КодСтеклопакета = model.КодСтеклопакета.Value;
                товар.Окна.Ширина = model.Ширина.Value;
                товар.Окна.Высота = model.Высота.Value;
                товар.Окна.Стандартное = model.Стандартное;
                товар.Окна.БазоваяГарантияМесяцев = model.БазоваяГарантияМесяцев.Value;

                // Удаляем старые створки
                _context.Створки.RemoveRange(товар.Окна.Створки);
                await _context.SaveChangesAsync(); // Сохраняем удаление

                // Добавляем новые
                foreach (var s in model.Створки)
                {
                    _context.Створки.Add(new Створки
                    {
                        КодОкна = товар.КодТовара,
                        КодТипаСтворки = s.КодТипаСтворки,
                        НомерСтворки = s.НомерСтворки
                    });
                }
            }
            else if (товар.Комплектующие != null && model.КодТипаТовара == 2)
            {
                товар.Комплектующие.КодТипаКомплектующего = model.КодТипаКомплектующего.Value;
                товар.Комплектующие.КодМатериала = model.КодМатериала;
                товар.Комплектующие.ДлинаМм = model.ДлинаМм;
                товар.Комплектующие.ШиринаМм = model.ШиринаМм;
                товар.Комплектующие.ВесКг = model.ВесКг;
            }

            _context.Update(товар);
        }
        else
        {
            // Создание нового товара
            var товар = new Товары
            {
                КодТипаТовара = model.КодТипаТовара,
                Название = model.Название,
                Цена = model.Цена,
                Цвет = model.Цвет,
                Активный = model.Активный
            };

            // Сохраняем фото если загружено
            if (фотоФайл != null && фотоФайл.Length > 0)
            {
                var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadFolder);

                // Используем имя файла клиента
                var fileName = Path.GetFileName(фотоФайл.FileName);
                var filePath = Path.Combine(uploadFolder, fileName);

                // Делаем имя файла уникальным если такой уже существует
                var counter = 1;
                var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                var extension = Path.GetExtension(fileName);

                while (System.IO.File.Exists(filePath))
                {
                    fileName = $"{nameWithoutExt}_{counter}{extension}";
                    filePath = Path.Combine(uploadFolder, fileName);
                    counter++;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await фотоФайл.CopyToAsync(stream);
                }

                товар.Фото = fileName;
            }

            _context.Товары.Add(товар);
            await _context.SaveChangesAsync(); // Получаем ID

            if (model.КодТипаТовара == 1) // Окно
            {
                var окно = new Окна
                {
                    КодТовара = товар.КодТовара,
                    КодПрофиля = model.КодПрофиля.Value,
                    КодСтеклопакета = model.КодСтеклопакета.Value,
                    Ширина = model.Ширина.Value,
                    Высота = model.Высота.Value,
                    Стандартное = model.Стандартное,
                    БазоваяГарантияМесяцев = model.БазоваяГарантияМесяцев.Value
                };
                _context.Окна.Add(окно);
                await _context.SaveChangesAsync();

                // Добавляем створки
                foreach (var s in model.Створки)
                {
                    _context.Створки.Add(new Створки
                    {
                        КодОкна = товар.КодТовара,
                        КодТипаСтворки = s.КодТипаСтворки,
                        НомерСтворки = s.НомерСтворки
                    });
                }
            }
            else if (model.КодТипаТовара == 2) // Комплектующее
            {
                var комплект = new Комплектующие
                {
                    КодТовара = товар.КодТовара,
                    КодТипаКомплектующего = model.КодТипаКомплектующего.Value,
                    КодМатериала = model.КодМатериала,
                    ДлинаМм = model.ДлинаМм,
                    ШиринаМм = model.ШиринаМм,
                    ВесКг = model.ВесКг
                };
                _context.Комплектующие.Add(комплект);
            }
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = model.КодТовара.HasValue ? "Товар обновлён" : "Товар добавлен";
        return RedirectToAction(nameof(Products));
    }
    public async Task<IActionResult> Services()
    {
        ViewBag.ActivePage = "Services";
        var services = await _context.Услуги
            .Include(u => u.КодТипаУслугиNavigation)
            .OrderBy(u => u.Название)
            .ToListAsync();
        return View(services);
    }

    // Форма создания/редактирования
    public async Task<IActionResult> EditService(int? id = null)
    {
        ViewBag.ActivePage = "Services";
        var model = new ServiceEditModel();
        model.ТипыУслуг = await _context.ТипыУслуг.ToListAsync();

        if (id.HasValue)
        {
            var service = await _context.Услуги.FindAsync(id.Value);
            if (service == null) return NotFound();

            model.КодУслуги = service.КодУслуги;
            model.Название = service.Название;
            model.Цена = service.Цена;
            model.КодТипаУслуги = service.КодТипаУслуги;
            model.Описание = service.Описание;
            model.Активна = service.Активна;
        }

        return View(model);
    }

    // Сохранение
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveService(ServiceEditModel model)
    {
        model.ТипыУслуг = await _context.ТипыУслуг.ToListAsync();

        if (!ModelState.IsValid)
        {
            return View("EditService", model);
        }

        if (model.КодУслуги.HasValue)
        {
            // Редактирование
            var service = await _context.Услуги.FindAsync(model.КодУслуги.Value);
            if (service == null) return NotFound();

            service.Название = model.Название;
            service.Цена = model.Цена;
            service.КодТипаУслуги = model.КодТипаУслуги;
            service.Описание = model.Описание;
            service.Активна = model.Активна;

            _context.Update(service);
        }
        else
        {
            // Создание
            var service = new Услуги
            {
                Название = model.Название,
                Цена = model.Цена,
                КодТипаУслуги = model.КодТипаУслуги,
                Описание = model.Описание,
                Активна = model.Активна
            };
            _context.Услуги.Add(service);
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = model.КодУслуги.HasValue ? "Услуга обновлена" : "Услуга добавлена";
        return RedirectToAction(nameof(Services));
    }
    public async Task<IActionResult> Orders(string search = "", int statusFilter = 0, string sortFilter = "newest")
    {
        var allStatuses = await _context.СтатусыЗаказа.ToListAsync();
        IQueryable<Заказы> orderQuery = _context.Заказы
                .Include(z => z.КодКлиентаNavigation)
                .Include(z => z.КодСтатусаЗаказаNavigation);
        if (statusFilter > 0)
        {
            orderQuery = orderQuery.Where(z => z.КодСтатусаЗаказа == statusFilter);
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
        ViewBag.ActivePage = "Orders";
        return View(viewModel);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        // Используем тот же сервис!
        var viewModel = await _orderDetailsService.BuildOrderDetailsViewModel(id);

        if (viewModel == null) return NotFound();

        return View(viewModel);
    }
}