// OKNODOM/DTOs/ProductEditModel.cs
using OKNODOM.Models;

namespace OKNODOM.DTOs;

public class ProductEditModel
{
    public int? КодТовара { get; set; }
    public int КодТипаТовара { get; set; } // 1 = Окно, 2 = Комплектующее
    public string Название { get; set; } = null!;
    public decimal Цена { get; set; }
    public string? Цвет { get; set; }
    public string? Фото { get; set; }
    public bool Активный { get; set; }

    // === Поля для окон ===
    public int? КодПрофиля { get; set; }
    public int? КодСтеклопакета { get; set; }
    public int? Ширина { get; set; }
    public int? Высота { get; set; }
    public bool Стандартное { get; set; }
    public int КоличествоСтворок { get; set; }
    public int? БазоваяГарантияМесяцев { get; set; }

    // Створки (вместо КоличествоСтворок)
    public List<WindowSash> Створки { get; set; } = new();

    // === Поля для комплектующих ===
    public int? КодТипаКомплектующего { get; set; }
    public int? КодМатериала { get; set; }
    public int? ДлинаМм { get; set; }
    public int? ШиринаМм { get; set; }
    public decimal? ВесКг { get; set; }

    // === Справочники (вместо ViewBag) ===
    public List<Профили> Профили { get; set; } = new();
    public List<Стеклопакеты> Стеклопакеты { get; set; } = new();
    public List<ТипыКомплектующих> ТипыКомплектующих { get; set; } = new();
    public List<Материалы> Материалы { get; set; } = new();
    public List<ТипыСтворок> ТипыСтворок { get; set; } = new();
    public List<ТипыТоваров> ТипыТоваров { get; set; } = new();

    // Вложенный класс
    public class WindowSash
    {
        public int? КодСтворки { get; set; }
        public int КодТипаСтворки { get; set; }
        public int НомерСтворки { get; set; }
    }
}