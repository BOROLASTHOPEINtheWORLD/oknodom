using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class ОконныеПроемы
{
    public int КодПроема { get; set; }

    public int КодЗамера { get; set; }

    public decimal Высота { get; set; }

    public decimal Ширина { get; set; }
    public int Этаж { get; set; }
    public string? Описание { get; set; }


    public virtual ICollection<ВыполнениеМонтажа> ВыполнениеМонтажаs { get; set; } = new List<ВыполнениеМонтажа>();

    public virtual Замеры КодЗамераNavigation { get; set; } = null!;

    public virtual ICollection<ТоварыВЗаказе> ТоварыВЗаказеs { get; set; } = new List<ТоварыВЗаказе>();
}
