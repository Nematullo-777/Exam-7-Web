using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MvcOnlineCourses.ViewModels;

public class EditCourseViewModel
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public CourseLevel Level { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}
