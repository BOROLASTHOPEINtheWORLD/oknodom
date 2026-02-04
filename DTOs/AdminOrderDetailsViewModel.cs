using OKNODOM.Models;

namespace OKNODOM.DTOs
{
    public class AdminOrderDetailsViewModel
    {
        public Заказы Заказ { get; set; }
        public Пользователи Клиент { get; set; }
        public Замеры ТекущийЗамер { get; set; }
        public List<ТоварыВЗаказе> ТекущиеТовары { get; set; } = new();
        public List<УслугиВЗаказе> ТекущиеУслуги { get; set; } = new();
        public List<ПозицияМонтажа> Позиции { get; set; }
        public List<Пользователи> НазначенныеМонтажники { get; set; }
        public List<Пользователи> Замерщики { get; set; } = new();
        public List<Пользователи> Бригадиры { get; set; } = new();
        public decimal СуммаТоваров { get; set; }
        public decimal СуммаУслуг { get; set; }
        public decimal ОбщаяСумма { get; set; }
    }
}
