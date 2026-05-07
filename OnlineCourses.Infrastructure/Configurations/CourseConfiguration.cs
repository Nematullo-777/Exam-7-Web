using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCourses.Domain.Entities;
using OnlineCourses.Domain.Identity;

namespace OnlineCourses.Infrastructure.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
        .IsRequired()
        .HasMaxLength(150);

        builder.Property(c => c.Description)
        .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
        .HasDefaultValueSql("NOW()");

        builder.HasOne(c => c.Category)
        .WithMany(ct => ct.Courses)
        .HasForeignKey(c => c.CategoryId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Instructor)
        .WithMany()
        .HasForeignKey(c => c.InstructorId)
        .OnDelete(DeleteBehavior.Cascade);        
    }
}
