using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Комплектующие
{
    public int КодТовара { get; set; }

    public int КодТипаКомплектующего { get; set; }

    public int? КодМатериала { get; set; }

    public int? ДлинаМм { get; set; }

    public int? ШиринаМм { get; set; }

    public int? ВысотаМм { get; set; }

    public decimal? ВесКг { get; set; }

    public virtual Товары КодКомплектующегоNavigation { get; set; } = null!;

    public virtual Материалы? КодМатериалаNavigation { get; set; }

    public virtual ТипыКомплектующих КодТипаКомплектующегоNavigation { get; set; } = null!;
}
