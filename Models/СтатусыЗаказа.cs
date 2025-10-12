using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class СтатусыЗаказа
{
    public int КодСтатусаЗаказа { get; set; }

    public string Название { get; set; } = null!;

    public virtual ICollection<Заказы> Заказыs { get; set; } = new List<Заказы>();
}
