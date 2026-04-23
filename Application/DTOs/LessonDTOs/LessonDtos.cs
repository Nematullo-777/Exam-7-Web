using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.LessonDTOs;

public class LessonDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public int DurationMinutes { get; set; }
    public Guid CourseId { get; set; }
}

public class CreateLessonDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public int DurationMinutes { get; set; }
}

public class UpdateLessonDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public int DurationMinutes { get; set; }
}
