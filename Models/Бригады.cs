using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Бригады
{
    public int Код { get; set; }

    public int КодВыполненияМонтажа { get; set; }

    public int КодМонтажника { get; set; }

    public virtual ВыполнениеМонтажа КодВыполненияМонтажаNavigation { get; set; } = null!;

    public virtual Пользователи КодМонтажникаNavigation { get; set; } = null!;
}
