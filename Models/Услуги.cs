using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Услуги")]
public partial class Услуги
{
    [Key]
    [Column("код_услуги")]
    public int КодУслуги { get; set; }

    [Column("название")]
    [StringLength(255)]
    public string Название { get; set; } = null!;

    [Column("базовая_стоимость", TypeName = "decimal(10, 2)")]
    public decimal БазоваяСтоимость { get; set; }

    [Column("код_типа_услуги")]
    public int КодТипаУслуги { get; set; }

    [Column("описание")]
    public string? Описание { get; set; }

    [Column("активна")]
    public bool Активна { get; set; }

    [ForeignKey("КодТипаУслуги")]
    public virtual ТипыУслуг КодТипаУслугиNavigation { get; set; } = null!;

    [InverseProperty("КодУслугиNavigation")]
    public virtual ICollection<УслугиВЗаказе> УслугиВЗаказе { get; set; } = new List<УслугиВЗаказе>();
}
