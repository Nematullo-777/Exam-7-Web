using System;
using OnlineCourses.Domain.Entities;

namespace OnlineCourses.Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<List<Review>> GetByCourseIdAsync(Guid courseId);
    Task<Review?> GetByIdAsync(Guid id);
    Task<Review?> GetByStudentAndCourseAsync(string studentId, Guid courseId);
    Task AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(Review review);
}
