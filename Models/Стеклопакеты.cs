using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Стеклопакеты
{
    public int КодСтеклопакета { get; set; }

    public string Название { get; set; } = null!;

    public virtual ICollection<Окна> Окнаs { get; set; } = new List<Окна>();
}
