using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class УслугиВЗаказе
{
    public int Код { get; set; }

    public int КодЗаказа { get; set; }

    public int КодУслуги { get; set; }

    public int? КодТовараВЗаказе { get; set; }

    public int Количество { get; set; }

    public decimal ЦенаНаМоментЗаказа { get; set; }

    public string? Примечание { get; set; }

    public virtual Заказы КодЗаказаNavigation { get; set; } = null!;

    public virtual ТоварыВЗаказе? КодТовараВЗаказеNavigation { get; set; }

    public virtual Услуги КодУслугиNavigation { get; set; } = null!;
}
