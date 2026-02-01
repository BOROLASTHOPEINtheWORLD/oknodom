using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Пользователи")]
[Index("Логин", Name = "UX_Пользователи_Логин", IsUnique = true)]
[Index("Телефон", Name = "UX_Пользователи_Телефон", IsUnique = true)]
public partial class Пользователи
{
    [Key]
    [Column("код_пользователя")]
    public int КодПользователя { get; set; }

    [Column("код_роли")]
    public int КодРоли { get; set; }

    [Column("фамилия")]
    [StringLength(100)]
    public string Фамилия { get; set; } = null!;

    [Column("имя")]
    [StringLength(100)]
    public string Имя { get; set; } = null!;

    [Column("отчество")]
    [StringLength(100)]
    public string? Отчество { get; set; }

    [Column("логин")]
    [StringLength(50)]
    public string Логин { get; set; } = null!;

    [Column("пароль")]
    [StringLength(255)]
    public string Пароль { get; set; } = null!;

    [Column("паспорт")]
    [StringLength(12)]
    public string? Паспорт { get; set; }

    [Column("телефон")]
    [StringLength(20)]
    public string Телефон { get; set; } = null!;

    [Column("активный")]
    public bool? Активный { get; set; }

    [InverseProperty("КодМонтажникаNavigation")]
    public virtual ICollection<Бригады> Бригады { get; set; } = new List<Бригады>();

    [InverseProperty("КодКлиентаNavigation")]
    public virtual ICollection<Заказы> Заказы { get; set; } = new List<Заказы>();

    [InverseProperty("КодЗамерщикаNavigation")]
    public virtual ICollection<Замеры> Замеры { get; set; } = new List<Замеры>();

    [ForeignKey("КодРоли")]
    public virtual Роли КодРолиNavigation { get; set; } = null!;
}
