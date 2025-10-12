using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Заказы
{
    public int КодЗаказа { get; set; }

    public int КодКлиента { get; set; }

    public int КодСтатусаЗаказа { get; set; }

    public DateTime ДатаСозданияЗаказа { get; set; }

    public bool СтатусОплаты { get; set; }

    public decimal? Стоимость { get; set; }

    public string Адрес { get; set; } = null!;

    public virtual ICollection<ВыполнениеМонтажа> ВыполнениеМонтажаs { get; set; } = new List<ВыполнениеМонтажа>();

    public virtual ICollection<Замеры> Замерыs { get; set; } = new List<Замеры>();

    public virtual Пользователи КодКлиентаNavigation { get; set; } = null!;

    public virtual СтатусыЗаказа КодСтатусаЗаказаNavigation { get; set; } = null!;

    public virtual ICollection<ТоварыВЗаказе> ТоварыВЗаказеs { get; set; } = new List<ТоварыВЗаказе>();

    public virtual ICollection<УслугиВЗаказе> УслугиВЗаказеs { get; set; } = new List<УслугиВЗаказе>();
}
