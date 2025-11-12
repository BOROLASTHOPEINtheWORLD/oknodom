namespace OKNODOM.DTOs.MeasurerDto
{
    public class MeasurerOrderViewModel
    {
        public int КодЗаказа {  get; set; }
        public string Адрес { get; set; } = null!;
        public string ФиоКлиента { get; set; } = null!;
        public string Телефон { get; set; } = null!;
        public string Статус { get; set; } = null!;
        public DateTime? ДатаЗамера {  get; set; }
    }
}
