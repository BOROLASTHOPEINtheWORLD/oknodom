using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Профили
{
    public int КодПрофиля { get; set; }

    public string Название { get; set; } = null!;

    public int Ширина { get; set; }

    public int? КоличествоКамер { get; set; }

    public int? СопротивлениеТеплопередаче { get; set; }

    public int? Звукоизоляция { get; set; }

    public int? ТолщинаСтеклопакета { get; set; }

    public virtual ICollection<Окна> Окнаs { get; set; } = new List<Окна>();
}
