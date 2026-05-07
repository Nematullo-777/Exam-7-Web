using System.ComponentModel.DataAnnotations;

namespace MvcOnlineCourses.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Пароль или Email не правильный")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль или Email не правильный")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
