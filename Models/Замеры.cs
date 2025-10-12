using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Замеры
{
    public int КодЗамера { get; set; }

    public int КодЗамерщика { get; set; }

    public int КодЗаказа { get; set; }

    public DateOnly? ДатаЗамера { get; set; }

    public virtual Заказы КодЗаказаNavigation { get; set; } = null!;

    public virtual Пользователи КодЗамерщикаNavigation { get; set; } = null!;

    public virtual ICollection<ОконныеПроемы> ОконныеПроемыs { get; set; } = new List<ОконныеПроемы>();
}
