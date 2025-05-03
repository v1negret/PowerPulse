using System.ComponentModel.DataAnnotations;

namespace PowerPulse.Modules.Authentication.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Поле 'Псевдоним' является обязательным для заполнения")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Поле 'Пароль' является обязательным для заполнения")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}