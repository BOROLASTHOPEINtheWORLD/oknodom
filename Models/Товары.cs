using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Товары")]
public partial class Товары
{
    [Key]
    [Column("код_товара")]
    public int КодТовара { get; set; }

    [Column("код_типа_товара")]
    public int КодТипаТовара { get; set; }

    [Column("название")]
    [StringLength(255)]
    public string Название { get; set; } = null!;

    [Column("цена", TypeName = "decimal(10, 2)")]
    public decimal Цена { get; set; }

    [Column("цвет")]
    [StringLength(50)]
    public string? Цвет { get; set; }

    [Column("фото")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Фото { get; set; }

    [Column("активный")]
    public bool Активный { get; set; }

    [ForeignKey("КодТипаТовара")]
    public virtual ТипыТоваров КодТипаТовараNavigation { get; set; } = null!;

    [InverseProperty("КодТовараNavigation")]
    public virtual Комплектующие? Комплектующие { get; set; }

    [InverseProperty("КодТовараNavigation")]
    public virtual Окна? Окна { get; set; }

    [InverseProperty("КодТовараNavigation")]
    public virtual ICollection<ТоварыВЗаказе> ТоварыВЗаказе { get; set; } = new List<ТоварыВЗаказе>();
}
