using System;
using OnlineCourses.Domain.Identity;

namespace OnlineCourses.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CraetedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string StudentId { get; set; } = string.Empty;
    public AppUser Student { get; set; } = null!;
}
