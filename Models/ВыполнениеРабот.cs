using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Выполнение_работ")]
public partial class ВыполнениеРабот
{
    [Key]
    [Column("код_выполнения")]
    public int КодВыполнения { get; set; }

    [Column("код_товара_в_заказе")]
    public int? КодТовараВЗаказе { get; set; }

    [Column("фотография")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Фотография { get; set; }

    [Column("дата_выполнения", TypeName = "datetime")]
    public DateTime? ДатаВыполнения { get; set; }

    public virtual ICollection<Бригады> Бригады { get; set; } = new List<Бригады>();


    [ForeignKey("КодТовараВЗаказе")]
    public virtual ТоварыВЗаказе? КодТовараВЗаказеNavigation { get; set; }

}
