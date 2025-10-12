using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Услуги
{
    public int КодУслуги { get; set; }

    public string Название { get; set; } = null!;

    public decimal БазоваяСтоимость { get; set; }

    public int КодТипаУслуги { get; set; }

    public string? Описание { get; set; }

    public bool Активна { get; set; }

    public virtual ТипыУслуг КодТипаУслугиNavigation { get; set; } = null!;

    public virtual ICollection<УслугиВЗаказе> УслугиВЗаказеs { get; set; } = new List<УслугиВЗаказе>();
}
