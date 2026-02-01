using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Статусы_заказа")]
public partial class СтатусыЗаказа
{
    [Key]
    [Column("код_статуса_заказа")]
    public int КодСтатусаЗаказа { get; set; }

    [Column("название")]
    [StringLength(100)]
    public string Название { get; set; } = null!;

    [InverseProperty("КодСтатусаЗаказаNavigation")]
    public virtual ICollection<Заказы> Заказы { get; set; } = new List<Заказы>();
}
