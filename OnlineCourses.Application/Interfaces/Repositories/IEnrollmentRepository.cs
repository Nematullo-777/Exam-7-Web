using System;
using OnlineCourses.Domain.Entities;

namespace OnlineCourses.Application.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<List<Enrollment>> GetAllAsync();
    Task<List<Enrollment>> GetByStudentIdAsync(string studentId);
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, Guid courseId);
    Task AddAsync(Enrollment enrollment);
    Task UpdateAsync(Enrollment enrollment);
    Task DeleteAsync(Enrollment enrollment);
}
