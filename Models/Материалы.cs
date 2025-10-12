using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Материалы
{
    public int КодМатериала { get; set; }

    public string Название { get; set; } = null!;

    public string? Описание { get; set; }

    public virtual ICollection<Комплектующие> Комплектующиеs { get; set; } = new List<Комплектующие>();
}
