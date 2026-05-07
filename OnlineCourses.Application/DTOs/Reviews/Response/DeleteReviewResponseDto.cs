using System;

namespace OnlineCourses.Application.DTOs.Reviews.Response;

public class DeleteReviewResponseDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
