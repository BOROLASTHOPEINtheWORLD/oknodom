using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Комплектующие")]
public partial class Комплектующие
{
    [Key]
    [Column("код_товара")]
    public int КодТовара { get; set; }

    [Column("код_типа_комплектующего")]
    public int КодТипаКомплектующего { get; set; }

    [Column("код_материала")]
    public int? КодМатериала { get; set; }

    [Column("длина_мм")]
    public int? ДлинаМм { get; set; }

    [Column("ширина_мм")]
    public int? ШиринаМм { get; set; }

    [Column("вес_кг", TypeName = "decimal(5, 2)")]
    public decimal? ВесКг { get; set; }

    [ForeignKey("КодМатериала")]
    public virtual Материалы? КодМатериалаNavigation { get; set; }

    [ForeignKey("КодТипаКомплектующего")]
    public virtual ТипыКомплектующих КодТипаКомплектующегоNavigation { get; set; } = null!;

    [ForeignKey("КодТовара")]
    [InverseProperty("Комплектующие")]
    public virtual Товары КодТовараNavigation { get; set; } = null!;
}
