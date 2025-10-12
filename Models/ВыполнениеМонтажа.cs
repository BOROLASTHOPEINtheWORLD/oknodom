using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class ВыполнениеМонтажа
{
    public int КодВыполнения { get; set; }

    public int КодЗаказа { get; set; }

    public int? КодОконногоПроема { get; set; }

    public int? КодУстановленногоОкна { get; set; }

    public string? Фотография { get; set; }

    public DateTime? ДатаВыполнения { get; set; }

    public virtual ICollection<Бригады> Бригадыs { get; set; } = new List<Бригады>();

    public virtual ТоварыВЗаказе КодВыполненияNavigation { get; set; } = null!;

    public virtual Заказы КодЗаказаNavigation { get; set; } = null!;

    public virtual ОконныеПроемы? КодОконногоПроемаNavigation { get; set; }
}
