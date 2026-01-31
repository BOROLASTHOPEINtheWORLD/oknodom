namespace OKNODOM.DTOs
{
    public class OrderConfigurationViewModel
    {
        public int КодЗаказа { get; set; }

        public List<ProductsForConfiguration> ДоступныеТовары { get; set; } = new();
        public List<ServicesForConfiguration> ДоступныеУслуги { get; set; } = new();

        public Dictionary<int, int> ТекущиеТовары { get; set; } = new(); 
        public Dictionary<int, int> ТекущиеУслуги { get; set; } = new();
        public List<WindowOpeningForSelection> ДоступныеПроемы { get; set; } = new();
        public Dictionary<int, int?> ПривязкаОконКПроемам { get; set; } = new();
        public decimal ТекущаяОбщаяСумма =>
            ТекущиеТовары.Sum(t => ПолучитьЦенуТовара(t.Key) * t.Value) +
            ТекущиеУслуги.Sum(s => ПолучитьЦенуУслуги(s.Key) * s.Value);

        private decimal ПолучитьЦенуТовара(int кодТовара) =>
            ДоступныеТовары.FirstOrDefault(t => t.КодТовара == кодТовара)?.Цена ?? 0;

        private decimal ПолучитьЦенуУслуги(int кодУслуги) =>
            ДоступныеУслуги.FirstOrDefault(s => s.КодУслуги == кодУслуги)?.Цена ?? 0;

        public string ВыбранныйФильтр { get; set; } = "all";

        public List<ProductsForConfiguration> ОтфильтрованныеТовары
        {
            get
            {
                return ВыбранныйФильтр switch
                {
                    "window" => ДоступныеТовары.Where(p => p.ЯвляетсяОкном).ToList(),
                    "accessory" => ДоступныеТовары.Where(p => !p.ЯвляетсяОкном).ToList(),
                    _ => ДоступныеТовары
                };
            }
        }
    }
    public class WindowOpeningForSelection
    {
        public int КодПроема { get; set; }
        public int Этаж { get; set; }
        public int Ширина { get; set; }
        public int Высота { get; set; }
        public string Описание { get; set; } = "";
        public string НазваниеДляОтображения => $"Проём {КодПроема} ({Ширина}×{Высота} мм, {Этаж} эт.)";
    }
    public class ProductsForConfiguration
    {
        public int КодТовара { get; set; }
        public string Название { get; set; } = string.Empty;
        public decimal Цена { get; set; }
        public string? Цвет { get; set; }
        public bool ЯвляетсяОкном { get; set; }
        public string? Размеры { get; set; } 
        public int ГарантияМесяцев { get; set; } = 12;
        public WindowDetails? ДеталиОкна { get; set; }
        public string? Фото { get; set; }
        public string ФотоUrl => !string.IsNullOrEmpty(Фото)
          ? $"/images/products/{Фото}"
          : "/images/products/no-image.jpg";
    }

    public class ServicesForConfiguration
    {
        public int КодУслуги { get; set; }
        public string Название { get; set; } = string.Empty;
        public decimal Цена { get; set; }
        public string? Описание { get; set; }
    }
    public class WindowDetails
    {
        public int КоличествоСтворок { get; set; }
        public bool Стандартное { get; set; }

        // Информация о профиле
        public string? НазваниеПрофиля { get; set; }
        public int? ШиринаПрофиля { get; set; }
        public int? КоличествоКамер { get; set; }
        public int? СопротивлениеТеплопередаче { get; set; }
        public int? Звукоизоляция { get; set; }

        // Информация о стеклопакете
        public string? НазваниеСтеклопакета { get; set; }
        public int? ТолщинаСтеклопакета { get; set; }

        // Детали створок
        public List<SashInfo>? Створки { get; set; }
    }

    public class WindowItemDto
    {
        public int КодТовара {  get; set; }
        public int Количество { get; set; }
        public int КодПроема { get; set; }
    }
    public class SashInfo
    {
        public int НомерСтворки { get; set; }
        public string ТипСтворки { get; set; } = string.Empty;
        public string? Описание { get; set; }
    }
}