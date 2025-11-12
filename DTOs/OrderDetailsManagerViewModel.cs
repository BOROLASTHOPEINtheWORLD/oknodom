using OKNODOM.Models;

namespace OKNODOM.DTOs
{
    public class OrderDetailsManagerViewModel
    {
        public Заказы Заказ { get; set; } = null!;
        public IEnumerable<Пользователи> Замерщики { get; set; } 
        
    }
}
