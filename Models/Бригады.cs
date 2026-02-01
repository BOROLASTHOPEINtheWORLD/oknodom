using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Бригады")]
[Index("КодВыполненияМонтажа", "КодМонтажника", Name = "IX_бригады_код_выполнения_монтажа__код_монтажника")]
public partial class Бригады
{
    [Key]
    [Column("код")]
    public int Код { get; set; }

    [Column("код_выполнения_монтажа")]
    public int КодВыполненияМонтажа { get; set; }

    [Column("код_монтажника")]
    public int КодМонтажника { get; set; }

    [ForeignKey("КодВыполненияМонтажа")]
    
    public virtual ВыполнениеРабот КодВыполненияМонтажаNavigation { get; set; } = null!;

    [ForeignKey("КодМонтажника")]

    public virtual Пользователи КодМонтажникаNavigation { get; set; } = null!;
}
