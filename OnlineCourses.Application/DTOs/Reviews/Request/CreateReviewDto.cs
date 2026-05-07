using System;

namespace OnlineCourses.Application.DTOs.Reviews.Request;

public class CreateReviewDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
