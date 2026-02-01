using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

[Table("Заказы")]
[Index("КодСтатусаЗаказа", "ДатаСозданияЗаказа", Name = "IX_Заказы_Статус_Дата")]
[Index("КодКлиента", "Стоимость", Name = "ix_заказы_клиент_стоимость")]
public partial class Заказы
{
    [Key]
    [Column("код_заказа")]
    public int КодЗаказа { get; set; }

    [Column("код_клиента")]
    public int КодКлиента { get; set; }

    [Column("код_статуса_заказа")]
    public int КодСтатусаЗаказа { get; set; }

    [Column("дата_создания_заказа", TypeName = "datetime")]
    public DateTime ДатаСозданияЗаказа { get; set; }

    [Column("статус_оплаты")]
    public bool СтатусОплаты { get; set; }

    [Column("стоимость", TypeName = "decimal(18, 2)")]
    public decimal? Стоимость { get; set; }

    [Column("адрес")]
    public string Адрес { get; set; } = null!;

    [Column("примечание_к_заказу")]
    [StringLength(500)]
    public string? ПримечаниеКЗаказу { get; set; }

    [InverseProperty("КодЗаказаNavigation")]
    public virtual ICollection<Замеры> Замеры { get; set; } = new List<Замеры>();

    [ForeignKey("КодКлиента")]
    public virtual Пользователи КодКлиентаNavigation { get; set; } = null!;

    [ForeignKey("КодСтатусаЗаказа")]
    public virtual СтатусыЗаказа КодСтатусаЗаказаNavigation { get; set; } = null!;

    [InverseProperty("КодЗаказаNavigation")]
    public virtual ICollection<ТоварыВЗаказе> ТоварыВЗаказе { get; set; } = new List<ТоварыВЗаказе>();

    [InverseProperty("КодЗаказаNavigation")]
    public virtual ICollection<УслугиВЗаказе> УслугиВЗаказе { get; set; } = new List<УслугиВЗаказе>();
}
