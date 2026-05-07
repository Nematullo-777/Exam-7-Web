using System;

namespace OnlineCourses.Application.DTOs.Lessons.Response;

public class UpdateLessonResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public string? MaterialPath { get; set; }
    public Guid CourseId { get; set; }
}
