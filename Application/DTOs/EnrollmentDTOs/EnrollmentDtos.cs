using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.EnrollmentDTOs;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public EnrollmentStatus Status { get; set; }
    public int ProgressPercent { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public Guid StudentId { get; set; } = Guid.Empty;
    public string StudentName { get; set; } = string.Empty;
}

public class CreateEnrollmentDto
{
    [Required]
    public Guid CourseId { get; set; }
}

public class UpdateProgressDto
{
    [Range(0, 100)]
    public int ProgressPercent { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string StudentName { get; set; } = string.Empty;
}

public class CreateReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    public string Comment { get; set; } = string.Empty;
}

public class UpdateReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    public string Comment { get; set; } = string.Empty;
}
