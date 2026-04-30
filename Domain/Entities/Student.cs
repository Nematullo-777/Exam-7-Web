
namespace Domain.Entities;

public class Student
{ 
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public string UserId { get; set; }
        public List<Review> Reviews { get; set; }
        
}

