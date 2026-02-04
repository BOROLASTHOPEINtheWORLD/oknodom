using OKNODOM.Models;

namespace OKNODOM.DTOs
{
    public class ServiceEditModel
    {
        public int? КодУслуги { get; set; }
        public string Название { get; set; } = null!;
        public decimal БазоваяСтоимость { get; set; }
        public int КодТипаУслуги { get; set; }
        public string? Описание { get; set; }
        public bool Активна { get; set; }

        // Справочник типов услуг
        public List<ТипыУслуг> ТипыУслуг { get; set; } = new();
    }
}
