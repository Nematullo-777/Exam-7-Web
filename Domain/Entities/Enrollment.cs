using Domain.Enums;

namespace Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public int ProgressPercent { get; set; } // 0-100

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public Guid StudentId { get; set; } = Guid.Empty;
    public Student Student { get; set; } = null!;
}
