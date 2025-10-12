using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Створки
{
    public int КодСтворки { get; set; }

    public int КодОкна { get; set; }

    public int КодТипаСтворки { get; set; }

    public int НомерСтворки { get; set; }

    public virtual Окна КодОкнаNavigation { get; set; } = null!;

    public virtual ТипыСтворок КодТипаСтворкиNavigation { get; set; } = null!;
}
