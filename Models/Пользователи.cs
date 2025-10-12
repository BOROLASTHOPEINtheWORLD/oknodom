using System;
using System.Collections.Generic;

namespace OKNODOM.Models;

public partial class Пользователи
{
    public int КодПользователя { get; set; }

    public int КодРоли { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчество { get; set; }

    public string Логин { get; set; } = null!;

    public string Пароль { get; set; } = null!;

    public string? Паспорт { get; set; }

    public string Телефон { get; set; } = null!;

    public int? СтатусСотрудника { get; set; }

    public virtual ICollection<Бригады> Бригадыs { get; set; } = new List<Бригады>();

    public virtual ICollection<Заказы> Заказыs { get; set; } = new List<Заказы>();

    public virtual ICollection<Замеры> Замерыs { get; set; } = new List<Замеры>();

    public virtual Роли КодРолиNavigation { get; set; } = null!;

    public virtual СтатусыСотрудников? СтатусСотрудникаNavigation { get; set; }
}
