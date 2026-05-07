using System;

namespace OnlineCourses.Application.DTOs.Enrollments.Response;

public class DeleteEnrollmentResponseDto
{
    public Guid Id { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
}
