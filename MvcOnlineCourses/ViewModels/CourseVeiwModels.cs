using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MvcOnlineCourses.ViewModels;

public class CreateCourseViewModel
{
    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Описание обязательно")]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
    public decimal Price { get; set; }

    public CourseLevel Level { get; set; }

    [Required(ErrorMessage = "Категория обязательна")]
    public Guid CategoryId { get; set; }
}

public class EditCourseViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Описание обязательно")]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
    public decimal Price { get; set; }

    public CourseLevel Level { get; set; }

    [Required(ErrorMessage = "Категория обязательна")]
    public Guid CategoryId { get; set; }
}