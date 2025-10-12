using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class ТипыСтворок
{
    public int Код { get; set; }

    public string Название { get; set; } = null!;

    public string? Описание { get; set; }

    public virtual ICollection<Створки> Створкиs { get; set; } = new List<Створки>();
}
