using OKNODOM.Models;

namespace OKNODOM.DTOs
{
    public class OrderDetailsManagerViewModel
    {
        public Заказы Заказ { get; set; } = null!;
        public IEnumerable<Пользователи> Замерщики { get; set; } = new List<Пользователи>();
        public IEnumerable<ТоварыВЗаказе> ТекущиеТовары { get; set; } = new List<ТоварыВЗаказе>();
        public IEnumerable<УслугиВЗаказе> ТекущиеУслуги { get; set; } = new List<УслугиВЗаказе>();
        public List<Товары> Товары { get; set; } = new List<Товары>();
        public List<Услуги> Услуги { get; set; } = new List<Услуги>();
        public Пользователи Клиент { get; set; } = null!;
        public Замеры ТекущийЗамер { get; set; }
        public ВыполнениеРабот Монтаж { get; set; }
        public List<Пользователи> Бригадиры { get; set; } = new();
        public List<Пользователи> НазначенныеМонтажники { get; set; } = new();
        public decimal СуммаТоваров => ТекущиеТовары?.Sum(t => t.ЦенаНаМоментЗаказа * t.Количество) ?? 0;
        public decimal СуммаУслуг => ТекущиеУслуги?.Sum(u => u.ЦенаНаМоментЗаказа * u.Количество) ?? 0;
        public decimal ОбщаяСумма => СуммаТоваров + СуммаУслуг;
        public bool МожноКонфигурировать => Заказ?.КодСтатусаЗаказа == 3 || Заказ?.КодСтатусаЗаказа == 8 || Заказ?.КодСтатусаЗаказа == 3 || Заказ?.КодСтатусаЗаказа == 1;
    }
}
