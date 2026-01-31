using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class ТоварыВЗаказе
{
    public int Код { get; set; }

    public int КодЗаказа { get; set; }

    public int? КодОконногоПроема { get; set; }

    public int КодТовара { get; set; }

    public int Количество { get; set; }

    public decimal ЦенаНаМоментЗаказа { get; set; }
    public int ГарантияМесяцев {  get; set; }
    public DateOnly? ГарантияДо {  get; set; }

    public virtual ВыполнениеМонтажа? ВыполнениеМонтажа { get; set; }
    public virtual Заказы КодЗаказаNavigation { get; set; } = null!;
    public virtual ОконныеПроемы? КодОконногоПроемаNavigation { get; set; }
    public virtual Товары КодТовараNavigation { get; set; } = null!;
}
