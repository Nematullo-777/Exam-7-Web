using Application.Common;
using Application.DTOs.LessonDTOs;
using Application.DTOs.StudentDTOs;
using Application.DTOs.CategoryDTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services;

public interface ILessonService
{
    Task<Result<List<LessonDto>>> GetByCourseIdAsync(Guid courseId);
    Task<Result<LessonDto>> GetByIdAsync(Guid id);
    Task<Result<LessonDto>> CreateAsync(Guid courseId, string instructorId, CreateLessonDto dto);
    Task<Result<LessonDto>> UpdateAsync(Guid id, string instructorId, bool isAdmin, UpdateLessonDto dto);
    Task<Result<bool>> DeleteAsync(Guid id, string instructorId, bool isAdmin);
    Task<Result<string>> UploadMaterialAsync(Guid id, string instructorId, IFormFile file);
}

public interface IStudentService
{
    Task<Result<PagedResult<StudentDto>>> GetAllAsync(int page, int pageSize);
    Task<Result<StudentDto>> GetByIdAsync(string id);
    Task<Result<StudentDto>> UpdateAsync(string id, string requestUserId, bool isAdmin, UpdateStudentDto dto);
    Task<Result<bool>> DeleteAsync(string id);
}

public interface ICategoryService
{
    Task<Result<List<CategoryDto>>> GetAllAsync();
    Task<Result<CategoryDto>> GetByIdAsync(Guid id);
    Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto);
    Task<Result<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task<Result<bool>> DeleteAsync(Guid id);
}
