using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OKNODOM.Models;

public partial class Заказы
{
    public int КодЗаказа { get; set; }

    public int КодКлиента { get; set; }

    public int КодСтатусаЗаказа { get; set; }

    public DateTime ДатаСозданияЗаказа { get; set; }

    public bool СтатусОплаты { get; set; }

    public decimal? Стоимость { get; set; }

    public string Адрес { get; set; } = null!;
    [StringLength(500, ErrorMessage = "Превышено 500 символов")]
    public string? ПримечаниеКЗаказу { get; set; } 

    public virtual ICollection<ВыполнениеМонтажа> ВыполнениеМонтажа { get; set; } = new List<ВыполнениеМонтажа>();

    public virtual ICollection<Замеры> Замеры { get; set; } = new List<Замеры>();

    public virtual Пользователи КодКлиентаNavigation { get; set; } = null!;

    public virtual СтатусыЗаказа КодСтатусаЗаказаNavigation { get; set; } = null!;

    public virtual ICollection<ТоварыВЗаказе> ТоварыВЗаказе { get; set; } = new List<ТоварыВЗаказе>();

    public virtual ICollection<УслугиВЗаказе> УслугиВЗаказе { get; set; } = new List<УслугиВЗаказе>();
}
