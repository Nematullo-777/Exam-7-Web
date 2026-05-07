using System;
using Microsoft.EntityFrameworkCore;
using OnlineCourses.Application.Interfaces.Repositories;
using OnlineCourses.Domain.Entities;
using OnlineCourses.Infrastructure.Data;

namespace OnlineCourses.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Review>> GetByCourseIdAsync(Guid courseId) =>
        await _context.Reviews
            .Include(r => r.Course)
            .Include(r => r.Student)
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.CraetedAt)
            .ToListAsync();

    public async Task<Review?> GetByIdAsync(Guid id) =>
        await _context.Reviews
            .Include(r => r.Course)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Review?> GetByStudentAndCourseAsync(string studentId, Guid courseId) =>
        await _context.Reviews
            .FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == courseId);

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review review)
    {
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }
}