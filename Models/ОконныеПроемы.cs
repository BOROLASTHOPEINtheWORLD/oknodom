using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Оконные_проемы")]
public partial class ОконныеПроемы
{
    [Key]
    [Column("код_проема")]
    public int КодПроема { get; set; }

    [Column("код_замера")]
    public int КодЗамера { get; set; }

    [Column("высота")]
    public int Высота { get; set; }

    [Column("ширина")]
    public int Ширина { get; set; }

    [Column("описание")]
    public string? Описание { get; set; }

    [Column("этаж")]
    public int Этаж { get; set; }

    [ForeignKey("КодЗамера")]
    public virtual Замеры КодЗамераNavigation { get; set; } = null!;

    [InverseProperty("КодОконногоПроемаNavigation")]
    public virtual ICollection<ТоварыВЗаказе> ТоварыВЗаказе { get; set; } = new List<ТоварыВЗаказе>();
}
