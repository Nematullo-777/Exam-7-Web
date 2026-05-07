using System;

namespace OnlineCourses.Application.DTOs.Reviews.Response;

public class UpdateReviewResponseDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public Guid CourseId { get; set; }
}
