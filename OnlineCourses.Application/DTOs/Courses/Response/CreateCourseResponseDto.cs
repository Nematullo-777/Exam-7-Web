using System;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.DTOs.Courses.Response;

public class CreateCourseResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
