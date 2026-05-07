using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Lessons.Request;
using OnlineCourses.Application.DTOs.Lessons.Response;

namespace OnlineCourses.Application.Interfaces.Services;

public interface ILessonService
{
    Task<Result<List<GetLessonDto>>> GetLessonsByCourseIdAsync(Guid courseId);
    Task<Result<GetLessonDto?>> GetLessonByIdAsync(Guid id);
    Task<Result<CreateLessonResponseDto>> CreateLessonAsync(Guid courseId, CreateLessonDto request);
    Task<Result<UpdateLessonResponseDto>> UpdateLessonAsync(Guid id, UpdateLessonDto request);
    Task<Result<DeleteLessonResponseDto>> DeleteLessonAsync(Guid id);
}
