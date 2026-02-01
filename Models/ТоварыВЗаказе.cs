using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Товары_в_заказе")]
[Index("КодЗаказа", "КодТовара", Name = "IX_ТоварыВЗаказе_ЗаказТовар")]
public partial class ТоварыВЗаказе
{
    [Key]
    [Column("код")]
    public int Код { get; set; }

    [Column("код_заказа")]
    public int КодЗаказа { get; set; }

    [Column("код_оконного_проема")]
    public int? КодОконногоПроема { get; set; }

    [Column("код_товара")]
    public int КодТовара { get; set; }

    [Column("количество")]
    public int Количество { get; set; }

    [Column("цена_на_момент_заказа", TypeName = "decimal(10, 2)")]
    public decimal ЦенаНаМоментЗаказа { get; set; }

    [Column("гарантия_месяцев")]
    public int ГарантияМесяцев { get; set; }

    [Column("гарантия_до")]
    public DateOnly? ГарантияДо { get; set; }

    [InverseProperty("КодВыполненияNavigation")]
    public virtual ВыполнениеРабот? ВыполнениеРабот { get; set; }

    [ForeignKey("КодЗаказа")]
    public virtual Заказы КодЗаказаNavigation { get; set; } = null!;

    [ForeignKey("КодОконногоПроема")]
    public virtual ОконныеПроемы КодОконногоПроемаNavigation { get; set; } = null!;

    [ForeignKey("КодТовара")]
    public virtual Товары КодТовараNavigation { get; set; } = null!;
}
