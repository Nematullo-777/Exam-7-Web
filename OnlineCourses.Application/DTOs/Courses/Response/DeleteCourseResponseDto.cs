using System;

namespace OnlineCourses.Application.DTOs.Courses.Response;

public class DeleteCourseResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}
