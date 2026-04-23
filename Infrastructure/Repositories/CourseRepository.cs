using Application.Common;
using Application.DTOs.CourseDTOs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository(AppDbContext context) : ICourseRepository
{
    public async Task<PagedResult<Course>> GetAllAsync(CourseFilterDto filter)
    {
        var query = context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .Include(c => c.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(c => c.Title.Contains(filter.Search) || c.Description.Contains(filter.Search));

        if (filter.CategoryId.HasValue)
            query = query.Where(c => c.CategoryId == filter.CategoryId.Value);

        if (filter.Level.HasValue)
            query = query.Where(c => c.Level == filter.Level.Value);

        if (filter.MinPrice.HasValue)
            query = query.Where(c => c.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(c => c.Price <= filter.MaxPrice.Value);

        if (filter.IsPublished.HasValue)
            query = query.Where(c => c.IsPublished == filter.IsPublished.Value);

        query = filter.SortBy?.ToLower() switch
        {
            "price" => filter.SortDescending ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price),
            "rating" => filter.SortDescending
                ? query.OrderByDescending(c => c.Reviews.Average(r => (double?)r.Rating) ?? 0)
                : query.OrderBy(c => c.Reviews.Average(r => (double?)r.Rating) ?? 0),
            "createdat" => filter.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Course>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<Course?> GetByIdAsync(Guid id) =>
        await context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .Include(c => c.Reviews).ThenInclude(r => r.Student)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Course> CreateAsync(Course course)
    {
        context.Courses.Add(course);
        await context.SaveChangesAsync();
        return course;
    }

    public async Task<Course> UpdateAsync(Course course)
    {
        context.Courses.Update(course);
        await context.SaveChangesAsync();
        return course;
    }

    public async Task DeleteAsync(Course course)
    {
        context.Courses.Remove(course);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id) =>
        await context.Courses.AnyAsync(c => c.Id == id);
}
