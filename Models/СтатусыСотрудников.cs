using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class СтатусыСотрудников
{
    public int Код { get; set; }

    public string Название { get; set; } = null!;

    public virtual ICollection<Пользователи> Пользователиs { get; set; } = new List<Пользователи>();
}
