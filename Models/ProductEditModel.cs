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
    public int? КоличествоСтворок { get; set; }
    public bool Стандартное { get; set; }
    public int? БазоваяГарантияМесяцев { get; set; }

    // === Поля для комплектующих ===
    public int? КодТипаКомплектующего { get; set; }
    public int? КодМатериала { get; set; }
    public int? ДлинаМм { get; set; }
    public int? ШиринаМм { get; set; }
    public decimal? ВесКг { get; set; }
}