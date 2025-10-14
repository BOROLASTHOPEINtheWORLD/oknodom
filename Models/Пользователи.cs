using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKNODOM.Models;
[Table("Пользователи")]
public partial class Пользователи
{
    public int КодПользователя { get; set; }

    public int КодРоли { get; set; }

    public string Фамилия { get; set; } = null!;

    public string Имя { get; set; } = null!;

    public string? Отчество { get; set; }
    [Required(ErrorMessage = "Введите логин")]
    public string Логин { get; set; } = null!;
    [Required(ErrorMessage = "Введите пароль")]
    public string Пароль { get; set; } = null!;

    public string? Паспорт { get; set; }

    public string Телефон { get; set; } = null!;
    
    public bool? Активный { get; set; }

    public virtual ICollection<Бригады> Бригады { get; set; } = new List<Бригады>();

    public virtual ICollection<Заказы> Заказы { get; set; } = new List<Заказы>();

    public virtual ICollection<Замеры> Замеры { get; set; } = new List<Замеры>();

    public virtual Роли КодРолиNavigation { get; set; } = null!;
}
