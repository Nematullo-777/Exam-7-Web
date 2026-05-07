using System;

namespace OnlineCourses.Application.DTOs.Reviews.Request;

public class UpdateReviewDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
