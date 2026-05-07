using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Lessons.Request;
using OnlineCourses.Application.DTOs.Lessons.Response;
using OnlineCourses.Application.Interfaces.Repositories;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Entities;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICacheService _cacheService;

    public LessonService(
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        ICacheService cacheService)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<List<GetLessonDto>>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var cacheKey = $"lessons_{courseId}";
        var cached = await _cacheService.GetAsync<List<GetLessonDto>>(cacheKey);
        if (cached != null)
            return Result<List<GetLessonDto>>.Ok(cached);

        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);

        var result = lessons.Select(l => new GetLessonDto
        {
            Id = l.Id,
            Title = l.Title,
            Description = l.Description,
            VideoUrl = l.VideoUrl,
            Order = l.Order,
            MaterialPath = l.MaterialPath,
            CreatedAt = l.CreatedAt,
            CourseId = l.CourseId,
            CourseTitle = l.Course.Title
        }).ToList();

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

        return Result<List<GetLessonDto>>.Ok(result);
    }

    public async Task<Result<GetLessonDto?>> GetLessonByIdAsync(Guid id)
    {
        var cacheKey = $"lesson_{id}";
        var cached = await _cacheService.GetAsync<GetLessonDto>(cacheKey);
        if (cached != null)
            return Result<GetLessonDto?>.Ok(cached);

        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            return Result<GetLessonDto?>.Fail("Lesson not found", ErrorType.NotFound);

        var dto = new GetLessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Description = lesson.Description,
            VideoUrl = lesson.VideoUrl,
            Order = lesson.Order,
            MaterialPath = lesson.MaterialPath,
            CreatedAt = lesson.CreatedAt,
            CourseId = lesson.CourseId,
            CourseTitle = lesson.Course.Title
        };

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

        return Result<GetLessonDto?>.Ok(dto);
    }

    public async Task<Result<CreateLessonResponseDto>> CreateLessonAsync(Guid courseId, CreateLessonDto request)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return Result<CreateLessonResponseDto>.Fail("Course not found", ErrorType.NotFound);

            var lesson = new Lesson
            {
                Title = request.Title,
                Description = request.Description,
                VideoUrl = request.VideoUrl,
                Order = request.Order,
                MaterialPath = request.MaterialPath,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow
            };

            await _lessonRepository.AddAsync(lesson);

            await _cacheService.RemoveAsync($"lessons_{courseId}");

            return Result<CreateLessonResponseDto>.Ok(new CreateLessonResponseDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Order = lesson.Order,
                CourseId = lesson.CourseId,
                CreatedAt = lesson.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<CreateLessonResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<UpdateLessonResponseDto>> UpdateLessonAsync(Guid id, UpdateLessonDto request)
    {
        try
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
                return Result<UpdateLessonResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

            lesson.Title = request.Title;
            lesson.Description = request.Description;
            lesson.VideoUrl = request.VideoUrl;
            lesson.Order = request.Order;
            lesson.MaterialPath = request.MaterialPath;

            await _lessonRepository.UpdateAsync(lesson);

            await _cacheService.RemoveAsync($"lessons_{lesson.CourseId}");
            await _cacheService.RemoveAsync($"lesson_{id}");

            return Result<UpdateLessonResponseDto>.Ok(new UpdateLessonResponseDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Order = lesson.Order,
                MaterialPath = lesson.MaterialPath,
                CourseId = lesson.CourseId
            });
        }
        catch (Exception ex)
        {
            return Result<UpdateLessonResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<DeleteLessonResponseDto>> DeleteLessonAsync(Guid id)
    {
        try
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
                return Result<DeleteLessonResponseDto>.Fail("Lesson not found", ErrorType.NotFound);

            await _lessonRepository.DeleteAsync(lesson);

            await _cacheService.RemoveAsync($"lessons_{lesson.CourseId}");
            await _cacheService.RemoveAsync($"lesson_{id}");

            return Result<DeleteLessonResponseDto>.Ok(new DeleteLessonResponseDto
            {
                Id = lesson.Id,
                Title = lesson.Title
            });
        }
        catch (Exception ex)
        {
            return Result<DeleteLessonResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }
}
