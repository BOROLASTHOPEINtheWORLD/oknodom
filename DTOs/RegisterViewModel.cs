using System.ComponentModel.DataAnnotations;

namespace OKNODOM.DTOs;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Укажите фамилию")]
    [StringLength(100)]
    public string Фамилия { get; set; } = null!;

    [Required(ErrorMessage = "Укажите имя")]
    [StringLength(100)]
    public string Имя { get; set; } = null!;

    [StringLength(100)]
    public string? Отчество { get; set; }

    [Required(ErrorMessage = "Укажите логин")]
    [StringLength(50)]
    public string Логин { get; set; } = null!;

    [Required(ErrorMessage = "Укажите пароль")]
    [DataType(DataType.Password)]
    public string Пароль { get; set; } = null!;

    [Required(ErrorMessage = "Подтвердите пароль")]
    [DataType(DataType.Password)]
    [Compare("Пароль", ErrorMessage = "Пароли не совпадают")]
    public string ПодтверждениеПароля { get; set; } = null!;

    [Required(ErrorMessage = "Укажите телефон")]
    [StringLength(20)]
    public string Телефон { get; set; } = null!;
}