using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Окна
{
    public int КодТовара { get; set; }

    public int КодПрофиля { get; set; }

    public int КодСтеклопакета { get; set; }

    public decimal Ширина { get; set; }

    public decimal Высота { get; set; }

    public int КоличествоСтворок { get; set; }

    public bool Стандартное { get; set; }
    public int БазоваяГарантияМесяцев {  get; set; }

    public virtual Товары КодОкнаNavigation { get; set; } = null!;

    public virtual Профили КодПрофиляNavigation { get; set; } = null!;

    public virtual Стеклопакеты КодСтеклопакетаNavigation { get; set; } = null!;

    public virtual ICollection<Створки> Створкиs { get; set; } = new List<Створки>();
}
