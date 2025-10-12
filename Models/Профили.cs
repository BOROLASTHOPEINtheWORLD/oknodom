using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Профили
{
    public int КодПрофиля { get; set; }

    public string Название { get; set; } = null!;

    public decimal? Ширина { get; set; }

    public int? КоличествоКамер { get; set; }

    public decimal? СопротивлениеТеплопередаче { get; set; }

    public decimal? Звукоизоляция { get; set; }

    public decimal? ТолщинаСтеклопакета { get; set; }

    public virtual ICollection<Окна> Окнаs { get; set; } = new List<Окна>();
}
