using OnlineCourses.Domain.Enums;
using OnlineCourses.Domain.Identity;

namespace OnlineCourses.Domain.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailPath { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    // Navigation properties
    public Guid CategoryId { get; set; }
    public  Category Category { get; set; } = null!;
    public string InstructorId { get; set; } = string.Empty;
    public AppUser Instructor { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
