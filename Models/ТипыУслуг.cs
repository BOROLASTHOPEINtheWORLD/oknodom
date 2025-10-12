using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class ТипыУслуг
{
    public int КодТипаУслуги { get; set; }

    public string Название { get; set; } = null!;

    public string? Описание { get; set; }

    public virtual ICollection<Услуги> Услугиs { get; set; } = new List<Услуги>();
}
