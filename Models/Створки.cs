using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Створки")]
public partial class Створки
{
    [Key]
    [Column("код_створки")]
    public int КодСтворки { get; set; }

    [Column("код_окна")]
    public int КодОкна { get; set; }

    [Column("код_типа_створки")]
    public int КодТипаСтворки { get; set; }

    [Column("номер_створки")]
    public int НомерСтворки { get; set; }

    [ForeignKey("КодОкна")]
    public virtual Окна КодОкнаNavigation { get; set; } = null!;

    [ForeignKey("КодТипаСтворки")]
    public virtual ТипыСтворок КодТипаСтворкиNavigation { get; set; } = null!;
}
