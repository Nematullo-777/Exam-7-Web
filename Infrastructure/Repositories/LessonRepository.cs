using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LessonRepository(AppDbContext context) : ILessonRepository
{
    public async Task<List<Lesson>> GetByCourseIdAsync(Guid courseId) =>
        await context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();

    public async Task<Lesson?> GetByIdAsync(Guid id) =>
        await context.Lessons.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Lesson> CreateAsync(Lesson lesson)
    {
        context.Lessons.Add(lesson);
        await context.SaveChangesAsync();
        return lesson;
    }

    public async Task<Lesson> UpdateAsync(Lesson lesson)
    {
        context.Lessons.Update(lesson);
        await context.SaveChangesAsync();
        return lesson;
    }

    public async Task DeleteAsync(Lesson lesson)
    {
        context.Lessons.Remove(lesson);
        await context.SaveChangesAsync();
    }
}
