using OKNODOM.Models;

namespace OKNODOM.DTOs.MeasurerDto
{
    public class MeasurerDashboardViewModel
    {
        public List<MeasurerOrderViewModel> Заказы { get; set; }
        public DateTime? ДатаС { get; set; }
        public DateTime? ДатаПо { get; set; }
        public string АктивнаяВкладка { get; set; } = "active";
    }
}
