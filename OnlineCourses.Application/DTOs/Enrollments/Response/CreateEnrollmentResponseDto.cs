using System;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.DTOs.Enrollments.Response;

public class CreateEnrollmentResponseDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; }
}
