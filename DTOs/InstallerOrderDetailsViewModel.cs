using OKNODOM.Models;

namespace OKNODOM.DTOs
{
    public class InstallerOrderDetailsViewModel
    {
        public int КодЗаказа { get; set; }
        public string Адрес { get; set; } = string.Empty;
        public string ФиоКлиента { get; set; } = string.Empty;
        public string Телефон { get; set; } = string.Empty;

        public bool ЗаказЗавершён { get; set; }
        
        public List<Пользователи> НазначенныеМонтажники { get; set; } = new();

        public List<ПозицияМонтажа> Позиции { get; set; } = new();
    }

    public class ПозицияМонтажа
    {
        public int КодПозиции { get; set; }
        public string Наименование { get; set; } = string.Empty;
        public string Размеры { get; set; } = string.Empty;

        // 👇 Добавляем проём
        public string? ПроёмЭтаж { get; set; }
        public string? ПроёмРазмеры { get; set; }
        public string? ПроёмОписание { get; set; }
        public DateTime? ДатаВыполнения { get; set; }
        public bool Выполнен { get; set; }
        public string? Фотография { get; set; }
        public int КодВыполнения { get; set; }
    }
}