using OKNODOM.Models;

namespace OKNODOM.DTOs
{
    public class ManagerDashboardViewModel
    {
        public IEnumerable<Заказы> Заказы { get; set; } 
        public IEnumerable<СтатусыЗаказа> ВсеСтатусы { get; set; } 
        public int ВыбранныйСтатусКод {  get; set; }
        public string Сортировка { get; set; }
        public string Поиск { get; set; }
    }
}
