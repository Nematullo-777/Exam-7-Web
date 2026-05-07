using System;

namespace OnlineCourses.Application.DTOs.Lessons.Response;

public class DeleteLessonResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}
