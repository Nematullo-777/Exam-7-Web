using Application.Common;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<PagedResult<Enrollment>> GetAllAsync(int page, int pageSize);
    Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId);
    Task<Enrollment?> GetByIdAsync(Guid id);
    Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<Enrollment> UpdateAsync(Enrollment enrollment);
    Task DeleteAsync(Enrollment enrollment);
    Task<List<TopCourseDto>> GetTopCoursesAsync(int count);
}
