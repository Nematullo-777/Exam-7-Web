using System;

namespace OnlineCourses.Application.DTOs.Reviews.Response;

public class CreateReviewResponseDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CourseId { get; set; }
    public string StudentId { get; set; } = string.Empty;
}
