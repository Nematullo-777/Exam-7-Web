using System;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.DTOs.Courses.Request;

public class UpdateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public Guid CategoryId { get; set; }
}
