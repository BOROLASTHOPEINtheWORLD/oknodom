using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Типы_услуг")]
[Index("Название", Name = "UQ__Типы_усл__5ED3ECC696EADE5C", IsUnique = true)]
public partial class ТипыУслуг
{
    [Key]
    [Column("код_типа_услуги")]
    public int КодТипаУслуги { get; set; }

    [Column("название")]
    [StringLength(50)]
    public string Название { get; set; } = null!;

    [Column("описание")]
    [StringLength(255)]
    public string? Описание { get; set; }

    [InverseProperty("КодТипаУслугиNavigation")]
    public virtual ICollection<Услуги> Услуги { get; set; } = new List<Услуги>();
}
