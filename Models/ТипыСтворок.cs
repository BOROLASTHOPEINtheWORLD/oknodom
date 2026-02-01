using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Типы_створок")]
public partial class ТипыСтворок
{
    [Key]
    [Column("код")]
    public int Код { get; set; }

    [Column("название")]
    [StringLength(100)]
    public string Название { get; set; } = null!;

    [Column("описание")]
    public string? Описание { get; set; }

    [InverseProperty("КодТипаСтворкиNavigation")]
    public virtual ICollection<Створки> Створки { get; set; } = new List<Створки>();
}
