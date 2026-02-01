using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Замеры")]
public partial class Замеры
{
    [Key]
    [Column("код_замера")]
    public int КодЗамера { get; set; }

    [Column("код_замерщика")]
    public int КодЗамерщика { get; set; }

    [Column("код_заказа")]
    public int КодЗаказа { get; set; }

    [Column("дата_замера", TypeName = "datetime")]
    public DateTime? ДатаЗамера { get; set; }

    [Column("есть_лифт")]
    public bool? ЕстьЛифт { get; set; }

    [ForeignKey("КодЗаказа")]
    public virtual Заказы КодЗаказаNavigation { get; set; } = null!;

    [ForeignKey("КодЗамерщика")]
    public virtual Пользователи КодЗамерщикаNavigation { get; set; } = null!;

    [InverseProperty("КодЗамераNavigation")]
    public virtual ICollection<ОконныеПроемы> ОконныеПроемы { get; set; } = new List<ОконныеПроемы>();
}
