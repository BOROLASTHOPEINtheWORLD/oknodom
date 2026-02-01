using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Профили")]
public partial class Профили
{
    [Key]
    [Column("код_профиля")]
    public int КодПрофиля { get; set; }

    [Column("название")]
    [StringLength(100)]
    public string Название { get; set; } = null!;

    [Column("ширина")]
    public int? Ширина { get; set; }

    [Column("количество_камер")]
    public int? КоличествоКамер { get; set; }

    [Column("сопротивление_теплопередаче")]
    public int? СопротивлениеТеплопередаче { get; set; }

    [Column("звукоизоляция")]
    public int? Звукоизоляция { get; set; }

    [Column("толщина_стеклопакета")]
    public int? ТолщинаСтеклопакета { get; set; }

    [InverseProperty("КодПрофиляNavigation")]
    public virtual ICollection<Окна> Окна { get; set; } = new List<Окна>();
}
