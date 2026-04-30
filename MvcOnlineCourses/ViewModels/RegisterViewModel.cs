using System.ComponentModel.DataAnnotations;

namespace MvcOnlineCourses.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Имя обязательно")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Роль обязательна")]
    public string Role { get; set; } = "Student"; // Student or Instructor
}
