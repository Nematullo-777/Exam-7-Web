using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ILessonRepository
{
    Task<List<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task<Lesson?> GetByIdAsync(Guid id);
    Task<Lesson> CreateAsync(Lesson lesson);
    Task<Lesson> UpdateAsync(Lesson lesson);
    Task DeleteAsync(Lesson lesson);
}
