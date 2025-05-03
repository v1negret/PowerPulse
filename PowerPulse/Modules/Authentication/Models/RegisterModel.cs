using System.ComponentModel.DataAnnotations;

namespace PowerPulse.Modules.Authentication.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Поле 'Псевдоним' является обязательным для заполнения")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Поле 'Email' является обязательным для заполнения")]
    [EmailAddress(ErrorMessage = "Неверный формат Email")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Поле 'Пароль' является обязательным для заполнения")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Поле 'Подтвердите пароль' является обязательным для заполнения")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; }
}