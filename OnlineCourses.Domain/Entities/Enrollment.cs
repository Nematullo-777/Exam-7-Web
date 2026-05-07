using System;
using OnlineCourses.Domain.Enums;
using OnlineCourses.Domain.Identity;

namespace OnlineCourses.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public int ProgressPercent { get; set; }

    // Navigation properties
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string StudentId { get; set; } = string.Empty;
    public AppUser Student { get; set; } = null!;
}
