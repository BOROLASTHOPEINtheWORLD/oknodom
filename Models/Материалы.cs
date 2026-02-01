using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Материалы")]
[Index("Название", Name = "UQ__Материал__5ED3ECC60910AD9D", IsUnique = true)]
public partial class Материалы
{
    [Key]
    [Column("код_материала")]
    public int КодМатериала { get; set; }

    [Column("название")]
    [StringLength(100)]
    public string Название { get; set; } = null!;

    [Column("описание")]
    [StringLength(255)]
    public string? Описание { get; set; }

    [InverseProperty("КодМатериалаNavigation")]
    public virtual ICollection<Комплектующие> Комплектующие { get; set; } = new List<Комплектующие>();
}
