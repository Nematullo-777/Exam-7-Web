using System;
using Microsoft.AspNetCore.Http;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Courses.Request;
using OnlineCourses.Application.DTOs.Courses.Response;

namespace OnlineCourses.Application.Interfaces.Services;

public interface ICourseService
{
    Task<Result<List<GetCourseDto>>> GetAllCoursesAsync();
    Task<Result<GetCourseDto?>> GetCourseByIdAsync(Guid id);
    Task<Result<CreateCourseResponseDto>> CreateCourseAsync(CreateCourseDto request, string instructorId);
    Task<Result<UpdateCourseResponseDto>> UpdateCourseAsync(Guid id, UpdateCourseDto request);
    Task<Result<DeleteCourseResponseDto>> DeleteCourseAsync(Guid id);
    Task<Result<bool>> TogglePublishAsync(Guid id);
    Task<Result<string>> UploadThumbnailAsync(Guid courseId, IFormFile file, string instructorId);
}