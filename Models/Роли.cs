using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Роли")]
public partial class Роли
{
    [Key]
    [Column("код_роли")]
    public int КодРоли { get; set; }

    [Column("название")]
    [StringLength(50)]
    public string Название { get; set; } = null!;

    [InverseProperty("КодРолиNavigation")]
    public virtual ICollection<Пользователи> Пользователи { get; set; } = new List<Пользователи>();
}
