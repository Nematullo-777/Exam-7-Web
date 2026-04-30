namespace Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    public int Rating { get; set; } 
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public Guid StudentId { get; set; } = Guid.Empty;
    public Student Student { get; set; } = null!;
}
