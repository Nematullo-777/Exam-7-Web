using System.ComponentModel.DataAnnotations;

namespace MvcOnlineCourses.ViewModels;

public class CreateLessonViewModel
{
    public Guid CourseId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? VideoUrl { get; set; }

    public int Order { get; set; }

    public int DurationMinutes { get; set; }
}
