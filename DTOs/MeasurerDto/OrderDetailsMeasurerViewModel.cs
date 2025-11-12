using OKNODOM.Models;

namespace OKNODOM.DTOs.MeasurerDto
{
    public class OrderDetailsMeasurerViewModel
    {
        public int КодЗамера { get; set; }
        public int КодЗаказа { get; set; }
        public DateTime? ДатаЗамера { get; set; }
        public bool ЕстьЛифт {  get; set; }
        public List<WindowsOpeningViewModel> Проемы { get; set; }
        public string КлиентФИО = null!;
        public string Адрес = null!;
    }
    public class WindowsOpeningViewModel
    {
        public int? КодПроема { get; set; }
        public int Этаж { get; set; }
        public decimal Ширина { get; set; }
        public decimal Высота {  get; set; }
        public string? Описание { get; set; }
    }
    
}
