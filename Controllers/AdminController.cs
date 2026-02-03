using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OKNODOM.DTOs;
using OKNODOM.Models;

namespace OKNODOM.Controllers;

[Authorize(Roles = "Админ")]
public class AdminController : Controller
{
    private readonly OknodomDbContext _context;

    public AdminController(OknodomDbContext context)
    {
        _context = context;
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

    // Список товаров
    public async Task<IActionResult> Products(string search = "", int typeFilter = 0)
    {
        ViewBag.ActivePage = "Products";
        ViewBag.Search = search;
        ViewBag.TypeFilter = typeFilter;

        var query = _context.Товары
            .Include(t => t.КодТипаТовараNavigation)
            .AsQueryable();

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
            .OrderBy(t => t.Название)
            .ToListAsync();

        ViewBag.Types = await _context.ТипыТоваров.ToListAsync();
        return View(products);
    }

    // Форма создания
    public async Task<IActionResult> CreateProduct(int type)
    {
        var model = new ProductEditModel { КодТипаТовара = type };
        await LoadDropdowns(model);
        return View(model);
    }

    // Форма редактирования
    // Этот метод теперь обрабатывает и создание, и редактирование
    public async Task<IActionResult> EditProduct(int? id = null, int? type = null)
    {
        if (id.HasValue)
        {
            // Редактирование существующего
            var товар = await _context.Товары
                .Include(t => t.Окна)
                .Include(t => t.Комплектующие)
                .FirstOrDefaultAsync(t => t.КодТовара == id.Value);

            if (товар == null) return NotFound();

            var model = new ProductEditModel
            {
                КодТовара = товар.КодТовара,
                КодТипаТовара = товар.КодТипаТовара,
                Название = товар.Название,
                Цена = товар.Цена,
                Цвет = товар.Цвет,
                Фото = товар.Фото,
                Активный = товар.Активный
            };

            // Заполняем поля подтипов
            if (товар.Окна != null)
            {
                model.КодПрофиля = товар.Окна.КодПрофиля;
                model.КодСтеклопакета = товар.Окна.КодСтеклопакета;
                model.Ширина = товар.Окна.Ширина;
                model.Высота = товар.Окна.Высота;
                model.КоличествоСтворок = товар.Окна.КоличествоСтворок;
                model.Стандартное = товар.Окна.Стандартное;
                model.БазоваяГарантияМесяцев = товар.Окна.БазоваяГарантияМесяцев;
            }
            else if (товар.Комплектующие != null)
            {
                model.КодТипаКомплектующего = товар.Комплектующие.КодТипаКомплектующего;
                model.КодМатериала = товар.Комплектующие.КодМатериала;
                model.ДлинаМм = товар.Комплектующие.ДлинаМм;
                model.ШиринаМм = товар.Комплектующие.ШиринаМм;
                model.ВесКг = товар.Комплектующие.ВесКг;
            }

            await LoadDropdowns(model);
            return View(model);
        }
        else if (type.HasValue)
        {
            // Создание нового
            var model = new ProductEditModel { КодТипаТовара = type.Value };
            await LoadDropdowns(model);
            return View(model);
        }
        else
        {
            return BadRequest("Не указан ни ID, ни тип");
        }
    }

    // Сохранение
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProduct(ProductEditModel model)
    {
        await LoadDropdowns(model); // для повторного отображения при ошибке

        if (!ModelState.IsValid)
        {
            return View(model.КодТовара.HasValue ? "EditProduct" : "CreateProduct", model);
        }

        if (model.КодТовара.HasValue)
        {
            // Редактирование
            var товар = await _context.Товары
                .Include(t => t.Окна)
                .Include(t => t.Комплектующие)
                .FirstOrDefaultAsync(t => t.КодТовара == model.КодТовара.Value);

            if (товар == null) return NotFound();

            // Обновляем основные поля
            товар.Название = model.Название;
            товар.Цена = model.Цена;
            товар.Цвет = model.Цвет;
            товар.Активный = model.Активный;

            // Обновляем подтипы
            if (товар.Окна != null && model.КодТипаТовара == 1)
            {
                товар.Окна.КодПрофиля = model.КодПрофиля.Value;
                товар.Окна.КодСтеклопакета = model.КодСтеклопакета.Value;
                товар.Окна.Ширина = model.Ширина.Value;
                товар.Окна.Высота = model.Высота.Value;
                товар.Окна.КоличествоСтворок = model.КоличествоСтворок.Value;
                товар.Окна.Стандартное = model.Стандартное;
                товар.Окна.БазоваяГарантияМесяцев = model.БазоваяГарантияМесяцев.Value;
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
            // Создание
            var товар = new Товары
            {
                КодТипаТовара = model.КодТипаТовара,
                Название = model.Название,
                Цена = model.Цена,
                Цвет = model.Цвет,
                Активный = model.Активный
            };

            _context.Товары.Add(товар);
            await _context.SaveChangesAsync(); // Получаем ID

            // Создаем подтип
            if (model.КодТипаТовара == 1) // Окно
            {
                var окно = new Окна
                {
                    КодТовара = товар.КодТовара,
                    КодПрофиля = model.КодПрофиля.Value,
                    КодСтеклопакета = model.КодСтеклопакета.Value,
                    Ширина = model.Ширина.Value,
                    Высота = model.Высота.Value,
                    КоличествоСтворок = model.КоличествоСтворок.Value,
                    Стандартное = model.Стандартное,
                    БазоваяГарантияМесяцев = model.БазоваяГарантияМесяцев.Value
                };
                _context.Окна.Add(окно);
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

    // Загрузка справочников
    private async Task LoadDropdowns(ProductEditModel model)
    {
        ViewBag.Types = await _context.ТипыТоваров.ToListAsync();
        ViewBag.Profiles = await _context.Профили.ToListAsync();
        ViewBag.GlassUnits = await _context.Стеклопакеты.ToListAsync();
        ViewBag.AccessoryTypes = await _context.ТипыКомплектующих.ToListAsync();
        ViewBag.Materials = await _context.Материалы.ToListAsync();
    }
}