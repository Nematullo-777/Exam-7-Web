using Application.Common;
using Application.DTOs.CourseDTOs;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<PagedResult<Course>> GetAllAsync(CourseFilterDto filter);
    Task<Course?> GetByIdAsync(Guid id);
    Task<Course> CreateAsync(Course course);
    Task<Course> UpdateAsync(Course course);
    Task DeleteAsync(Course course);
    Task<bool> ExistsAsync(Guid id);
}
