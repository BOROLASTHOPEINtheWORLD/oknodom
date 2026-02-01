using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Окна")]
public partial class Окна
{
    [Key]
    [Column("код_товара")]
    public int КодТовара { get; set; }

    [Column("код_профиля")]
    public int КодПрофиля { get; set; }

    [Column("код_стеклопакета")]
    public int КодСтеклопакета { get; set; }

    [Column("ширина")]
    public int Ширина { get; set; }

    [Column("высота")]
    public int Высота { get; set; }

    [Column("количество_створок")]
    public int КоличествоСтворок { get; set; }

    [Column("стандартное")]
    public bool Стандартное { get; set; }

    [Column("базовая_гарантия_месяцев")]
    public int БазоваяГарантияМесяцев { get; set; }

    [ForeignKey("КодПрофиля")]
    public virtual Профили КодПрофиляNavigation { get; set; } = null!;

    [ForeignKey("КодСтеклопакета")]
    public virtual Стеклопакеты КодСтеклопакетаNavigation { get; set; } = null!;

    [ForeignKey("КодТовара")]
    [InverseProperty("Окна")]
    public virtual Товары КодТовараNavigation { get; set; } = null!;

    [InverseProperty("КодОкнаNavigation")]
    public virtual ICollection<Створки> Створки { get; set; } = new List<Створки>();
}
