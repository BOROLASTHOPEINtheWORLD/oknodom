using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Типы_комплектующих")]
[Index("Название", Name = "UQ__Типы_ком__5ED3ECC607BDB074", IsUnique = true)]
public partial class ТипыКомплектующих
{
    [Key]
    [Column("код_типа")]
    public int КодТипа { get; set; }

    [Column("название")]
    [StringLength(50)]
    public string Название { get; set; } = null!;

    [Column("описание")]
    [StringLength(255)]
    public string? Описание { get; set; }

    [InverseProperty("КодТипаКомплектующегоNavigation")]
    public virtual ICollection<Комплектующие> Комплектующие { get; set; } = new List<Комплектующие>();
}
