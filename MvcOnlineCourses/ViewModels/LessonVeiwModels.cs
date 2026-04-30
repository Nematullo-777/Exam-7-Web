using System.ComponentModel.DataAnnotations;

namespace MvcOnlineCourses.ViewModels;

public class CreateLessonViewModel
{
    public Guid CourseId { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Содержание обязательно")]
    public string Content { get; set; } = string.Empty;

    public string? VideoUrl { get; set; }

    public int Order { get; set; }

    public int DurationMinutes { get; set; }
}

public class EditLessonViewModel
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Содержание обязательно")]
    public string Content { get; set; } = string.Empty;

    public string? VideoUrl { get; set; }

    public int Order { get; set; }

    public int DurationMinutes { get; set; }
}