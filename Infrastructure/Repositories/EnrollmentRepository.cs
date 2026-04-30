using Application.Common;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EnrollmentRepository(AppDbContext context) : IEnrollmentRepository
{
    public async Task<PagedResult<Enrollment>> GetAllAsync(int page, int pageSize)
    {
        var query = context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Enrollment>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId) =>
        await context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

    public async Task<Enrollment?> GetByIdAsync(Guid id) =>
        await context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId) =>
        await context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
    {
        context.Enrollments.Update(enrollment);
        await context.SaveChangesAsync();
        return enrollment;
    }

    public async Task DeleteAsync(Enrollment enrollment)
    {
        context.Enrollments.Remove(enrollment);
        await context.SaveChangesAsync();
    }

    public async Task<List<TopCourseDto>> GetTopCoursesAsync(int count)
    {
        return await context.Enrollments
            .GroupBy(e => e.CourseId)
            .Select(g => new TopCourseDto
            {
                CourseId = g.Key,
                Title = g.First().Course.Title,
                InstructorName = g.First().Course.Instructor.FullName,
                EnrollmentCount = g.Count(),
                CompletedCount = g.Count(e => e.Status == EnrollmentStatus.Completed),
                CompletionRate = g.Count() > 0
                    ? (double)g.Count(e => e.Status == EnrollmentStatus.Completed) / g.Count() * 100
                    : 0,
                AverageRating = g.First().Course.Reviews.Any()
                    ? g.First().Course.Reviews.Average(r => (double)r.Rating)
                    : 0,
                Revenue = g.Count() * g.First().Course.Price
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(count)
            .ToListAsync();
    }
}
