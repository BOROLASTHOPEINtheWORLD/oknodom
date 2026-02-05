using OKNODOM.DTOs;
using OKNODOM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OKNODOM.Services
{
    public class OrderDetailsService
    {
        private readonly OknodomDbContext _context;

        public OrderDetailsService(OknodomDbContext context)
        {
            _context = context;
        }

        // Общий метод для всех ролей
        public async Task<AdminOrderDetailsViewModel> BuildOrderDetailsViewModel(int orderId)
        {
            var order = await _context.Заказы
                .Include(z => z.КодКлиентаNavigation)
                .Include(z => z.КодСтатусаЗаказаNavigation)
                .FirstOrDefaultAsync(z => z.КодЗаказа == orderId);

            if (order == null) return null;

            // Загружаем позиции товаров
            var позицииТоваров = await _context.ТоварыВЗаказе
                .Where(t => t.КодЗаказа == orderId)
                .Include(t => t.КодТовараNavigation)
                    .ThenInclude(tov => tov.Окна)
                .Include(t => t.КодОконногоПроемаNavigation)
                .Include(t => t.Выполнения)
                .ToListAsync();

            var услуги = await _context.УслугиВЗаказе
                .Where(u => u.КодЗаказа == orderId)
                .Include(u => u.КодУслугиNavigation)
                .ToListAsync();

            var замер = await _context.Замеры
                .Include(z => z.КодЗамерщикаNavigation)
                .Include(z => z.ОконныеПроемы)
                .FirstOrDefaultAsync(z => z.КодЗаказа == orderId);

            // Считаем суммы
            var суммаТоваров = позицииТоваров.Sum(t => t.ЦенаНаМоментЗаказа * t.Количество);
            var суммаУслуг = услуги.Sum(u => u.ЦенаНаМоментЗаказа * u.Количество);
            var общаяСумма = суммаТоваров + суммаУслуг;

            // Преобразуем позиции для монтажа
            var позицииМонтажа = позицииТоваров.Select(t => new ПозицияМонтажа
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
            }).ToList();

            return new AdminOrderDetailsViewModel
            {
                Заказ = order,
                Клиент = order.КодКлиентаNavigation,
                ТекущийЗамер = замер,
                ТекущиеТовары = позицииТоваров,
                ТекущиеУслуги = услуги,
                Позиции = позицииМонтажа,
                СуммаТоваров = суммаТоваров,
                СуммаУслуг = суммаУслуг,
                ОбщаяСумма = общаяСумма
            };
        }
    }
}