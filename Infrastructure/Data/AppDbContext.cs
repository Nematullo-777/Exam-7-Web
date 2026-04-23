using Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new CourseConfiguration());
        builder.ApplyConfiguration(new EnrollmentConfiguration());
        builder.ApplyConfiguration(new ReviewConfiguration());
        builder.ApplyConfiguration(new LessonConfiguration());
        builder.ApplyConfiguration(new CategoryConfiguration());
    }
}
