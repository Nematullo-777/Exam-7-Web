using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCourses.Domain.Entities;

namespace OnlineCourses.Infrastructure.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
        .IsRequired();

        builder.Property(r => r.Comment)
        .HasMaxLength(500);

        builder.Property(r => r.CraetedAt)
        .HasDefaultValueSql("NOW()");

        builder.HasOne(r => r.Course)
        .WithMany(c => c.Reviews)
        .HasForeignKey(r => r.CourseId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Student)
        .WithMany()
        .HasForeignKey(r => r.StudentId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
