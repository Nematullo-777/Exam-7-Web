using System;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.DTOs.Courses.Response;

public class GetCourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailPath { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int LessonCount { get; set; }
    public double AverageRating { get; set; }
}
