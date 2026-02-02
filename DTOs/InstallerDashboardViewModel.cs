namespace OKNODOM.DTOs
{
    public class InstallerDashboardViewModel
    {
        public List<InstallerOrderViewModel> Заказы { get; set; } = new();
        public string АктивнаяВкладка { get; set; } = "active";
    }
}
