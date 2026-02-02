namespace OKNODOM.DTOs
{
    public class InstallerOrderViewModel
    {
        public int КодЗаказа { get; set; }
        public string Адрес { get; set; } = string.Empty;
        public string ФиоКлиента { get; set; } = string.Empty;
        public string Телефон { get; set; } = string.Empty;
        public string Статус { get; set; } = string.Empty;
        public DateTime? ДатаНазначения { get; set; }
       
    }
}
