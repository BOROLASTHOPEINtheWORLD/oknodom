using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Товары
{
    public int КодТовара { get; set; }

    public int ТипТовара { get; set; }

    public string Название { get; set; } = null!;

    public decimal Цена { get; set; }

    public string? Цвет { get; set; }

    public string? Фото { get; set; }

    public bool Активный { get; set; }

    public virtual Комплектующие? Комплектующие { get; set; }

    public virtual Окна? Окна { get; set; }

    public virtual ТипыТоваров ТипТовараNavigation { get; set; } = null!;

    public virtual ICollection<ТоварыВЗаказе> ТоварыВЗаказе { get; set; } = new List<ТоварыВЗаказе>();
}
