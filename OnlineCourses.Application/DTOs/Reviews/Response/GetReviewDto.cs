using System;

namespace OnlineCourses.Application.DTOs.Reviews.Response;

public class GetReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
}
