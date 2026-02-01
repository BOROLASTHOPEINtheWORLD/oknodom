using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Услуги_в_заказе")]
[Index("КодЗаказа", Name = "IX_УслугиВЗаказе_Заказ")]
public partial class УслугиВЗаказе
{
    [Key]
    [Column("код")]
    public int Код { get; set; }

    [Column("код_заказа")]
    public int КодЗаказа { get; set; }

    [Column("код_услуги")]
    public int КодУслуги { get; set; }

    [Column("количество")]
    public int Количество { get; set; }

    [Column("цена_на_момент_заказа", TypeName = "decimal(10, 2)")]
    public decimal ЦенаНаМоментЗаказа { get; set; }

    [Column("примечание")]
    [StringLength(500)]
    public string? Примечание { get; set; }

    [ForeignKey("КодЗаказа")]
    public virtual Заказы КодЗаказаNavigation { get; set; } = null!;

    [ForeignKey("КодУслуги")]
    public virtual Услуги КодУслугиNavigation { get; set; } = null!;
}
