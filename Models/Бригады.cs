using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Бригады")]
public partial class Бригады
{
    [Key]
    [Column("код")]
    public int Код { get; set; }

    [Column("код_выполнения_работ")]
    public int КодВыполнения { get; set; } // ← имя свойства = КодВыполнения

    [Column("код_монтажника")]
    public int КодМонтажника { get; set; }

    [ForeignKey("КодВыполнения")]
    public virtual ВыполнениеРабот КодВыполненияNavigation { get; set; } = null!;

    [ForeignKey("КодМонтажника")]
    public virtual Пользователи КодМонтажникаNavigation { get; set; } = null!;
}