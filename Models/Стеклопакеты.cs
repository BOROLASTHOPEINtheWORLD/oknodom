using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Стеклопакеты")]
public partial class Стеклопакеты
{
    [Key]
    [Column("код_стеклопакета")]
    public int КодСтеклопакета { get; set; }

    [Column("название")]
    [StringLength(100)]
    public string Название { get; set; } = null!;

    [InverseProperty("КодСтеклопакетаNavigation")]
    public virtual ICollection<Окна> Окна { get; set; } = new List<Окна>();
}
