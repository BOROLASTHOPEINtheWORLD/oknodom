using System.ComponentModel.DataAnnotations;

namespace OKNODOM.DTOs
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [Display(Name = "Логин")]
        public string Логин { get; set; } = null!;
        [Required(ErrorMessage = "Введите Пароль")]
        [Display(Name = "Пароль")]
        public string Пароль { get; set; } = null!;
        [Display(Name = "Запомнить меня")]
        public bool ЗапомнитьМеня { get; set; }
    }
}
