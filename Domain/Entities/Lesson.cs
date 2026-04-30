namespace Domain.Entities;

public class Lesson
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public string? MaterialPath{ get; set; }
    public int Order { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
