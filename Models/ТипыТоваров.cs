using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Типы_товаров")]
public partial class ТипыТоваров
{
    [Key]
    [Column("код")]
    public int Код { get; set; }

    [Column("название")]
    [StringLength(100)]
    public string Название { get; set; } = null!;

    [InverseProperty("КодТипаТовараNavigation")]
    public virtual ICollection<Товары> Товары { get; set; } = new List<Товары>();
}
