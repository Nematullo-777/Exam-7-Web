using System;

namespace OnlineCourses.Application.DTOs.Lessons.Response;

public class CreateLessonResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public Guid CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
}
