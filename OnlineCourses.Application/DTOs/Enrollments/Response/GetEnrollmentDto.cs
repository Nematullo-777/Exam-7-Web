using System;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.DTOs.Enrollments.Response;

public class GetEnrollmentDto
{
    public Guid Id { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public EnrollmentStatus Status { get; set; }
    public int ProgressPercent { get; set; }

    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public decimal CoursePrice { get; set; }

    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
}
