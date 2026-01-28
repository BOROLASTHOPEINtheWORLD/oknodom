using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class ТипыТоваров
{
    public int Код { get; set; }

    public string Название { get; set; } = null!;

    public virtual ICollection<Товары> Товары { get; set; } = new List<Товары>();
}
