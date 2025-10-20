using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace OKNODOM.DTOs
{
    public class OrderViewModel
    {
        public int? КодПользователя {  get; set; }

        [Display(Name="ФИО клиента")]
        public string ПолноеИмя {  get; set; } = null!;
        [Display(Name = "Контактный телефон")]
        [Required(ErrorMessage = "Телефон обязателен")]
        public string Телефон { get; set; } = string.Empty;

        [Display(Name = "Адрес установки")]
        [Required(ErrorMessage = "Адрес обязателен")]
        public string Адрес { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Превышено 500 символов")]
        [Display(Name = "Описание работ (опционально)")]
        public string? Описание { get; set; }
    }
}
