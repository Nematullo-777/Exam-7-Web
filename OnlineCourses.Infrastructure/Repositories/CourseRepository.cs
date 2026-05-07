using System;
using Microsoft.EntityFrameworkCore;
using OnlineCourses.Application.Interfaces.Repositories;
using OnlineCourses.Domain.Entities;
using OnlineCourses.Infrastructure.Data;

namespace OnlineCourses.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Course>> GetAllAsync() =>
    await _context.Courses
        .Include(c => c.Category)
        .Include(c => c.Instructor)
        .Include(c => c.Lessons)
        .Include(c => c.Reviews)
        .ToListAsync();

    public async Task<Course?> GetByIdAsync(Guid id) =>
        await _context.Courses.FindAsync(id);
 
    public async Task<Course?> GetByIdWithDetailsAsync(Guid id) =>
        await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course course)
    {
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }
}