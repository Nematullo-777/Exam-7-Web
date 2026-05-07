using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCourses.Domain.Entities;

namespace OnlineCourses.Infrastructure.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
        .IsRequired()
        .HasMaxLength(150);

        builder.Property(l => l.Description)
        .HasMaxLength(500);

        builder.Property(l => l.Order)
        .IsRequired();

        builder.Property(l => l.CreatedAt)
        .HasDefaultValueSql("NOW()");

        builder.HasOne(l => l.Course)
        .WithMany(c => c.Lessons)
        .HasForeignKey(l => l.CourseId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
