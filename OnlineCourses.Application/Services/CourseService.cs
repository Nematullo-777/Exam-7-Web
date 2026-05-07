using System;
using Microsoft.AspNetCore.Http;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Courses.Request;
using OnlineCourses.Application.DTOs.Courses.Response;
using OnlineCourses.Application.Interfaces.Repositories;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Entities;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICacheService _cacheService;
    private readonly  IFileService _fileService;

    public CourseService(ICourseRepository courseRepository, ICacheService cacheService, IFileService fileService)
    {
        _courseRepository = courseRepository;
        _cacheService = cacheService;
        _fileService = fileService;
    }

    public async Task<Result<List<GetCourseDto>>> GetAllCoursesAsync()
    {
        var cached = await _cacheService.GetAsync<List<GetCourseDto>>("all_courses");
        if (cached != null)
            return Result<List<GetCourseDto>>.Ok(cached);

        var courses = await _courseRepository.GetAllAsync();

        var result = courses.Select(c => new GetCourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            ThumbnailPath = c.ThumbnailPath,
            Price = c.Price,
            Level = c.Level,
            IsPublished = c.IsPublished,
            CreatedAt = c.CreatedAt,
            CategoryName = c.Category.Name,
            InstructorName = c.Instructor.FullName,
            LessonCount = c.Lessons.Count,
            AverageRating = c.Reviews?.Any() == true
                ? c.Reviews.Average(r => r.Rating)
                : 0
        }).ToList();

        await _cacheService.SetAsync("all_courses", result, TimeSpan.FromMinutes(10));

        return Result<List<GetCourseDto>>.Ok(result);
    }

    public async Task<Result<GetCourseDto?>> GetCourseByIdAsync(Guid id)
    {
        var cached = await _cacheService.GetAsync<GetCourseDto>($"course_{id}");
        if (cached != null)
            return Result<GetCourseDto?>.Ok(cached);

        var course = await _courseRepository.GetByIdWithDetailsAsync(id);
        if (course == null)
            return Result<GetCourseDto?>.Fail("Course not found", ErrorType.NotFound);

        var dto = new GetCourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            ThumbnailPath = course.ThumbnailPath,
            Price = course.Price,
            Level = course.Level,
            IsPublished = course.IsPublished,
            CreatedAt = course.CreatedAt,
            CategoryName = course.Category.Name,
            InstructorName = course.Instructor.FullName,
            LessonCount = course.Lessons.Count,
            AverageRating = course.Reviews?.Any() == true
                ? course.Reviews.Average(r => r.Rating)
                : 0
        };

        await _cacheService.SetAsync($"course_{id}", dto, TimeSpan.FromMinutes(10));

        return Result<GetCourseDto?>.Ok(dto);
    }

    public async Task<Result<CreateCourseResponseDto>> CreateCourseAsync(CreateCourseDto request, string instructorId)
    {
        try
        {
            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                Level = request.Level,
                CategoryId = request.CategoryId,
                InstructorId = instructorId,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow
            };

            await _courseRepository.AddAsync(course);

            await _cacheService.RemoveAsync("all_courses");

            return Result<CreateCourseResponseDto>.Ok(new CreateCourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Price = course.Price,
                Level = course.Level,
                CategoryId = course.CategoryId,
                CreatedAt = course.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<CreateCourseResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<UpdateCourseResponseDto>> UpdateCourseAsync(
        Guid id, UpdateCourseDto request)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return Result<UpdateCourseResponseDto>.Fail("Course not found", ErrorType.NotFound);

            course.Title = request.Title;
            course.Description = request.Description;
            course.Price = request.Price;
            course.Level = request.Level;
            course.CategoryId = request.CategoryId;

            await _courseRepository.UpdateAsync(course);

            await _cacheService.RemoveAsync("all_courses");
            await _cacheService.RemoveAsync($"course_{id}");

            return Result<UpdateCourseResponseDto>.Ok(new UpdateCourseResponseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Level = course.Level,
                CategoryId = course.CategoryId
            });
        }
        catch (Exception ex)
        {
            return Result<UpdateCourseResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<DeleteCourseResponseDto>> DeleteCourseAsync(
        Guid id)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return Result<DeleteCourseResponseDto>.Fail("Course not found", ErrorType.NotFound);

            await _courseRepository.DeleteAsync(course);

            await _cacheService.RemoveAsync("all_courses");
            await _cacheService.RemoveAsync($"course_{id}");

            return Result<DeleteCourseResponseDto>.Ok(new DeleteCourseResponseDto
            {
                Id = course.Id,
                Title = course.Title
            });
        }
        catch (Exception ex)
        {
            return Result<DeleteCourseResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> TogglePublishAsync(Guid id)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                return Result<bool>.Fail("Course not found", ErrorType.NotFound);

            course.IsPublished = !course.IsPublished;

            await _courseRepository.UpdateAsync(course);

            await _cacheService.RemoveAsync("all_courses");
            await _cacheService.RemoveAsync($"course_{id}");

            return Result<bool>.Ok(course.IsPublished);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }
    
    public async Task<Result<string>> UploadThumbnailAsync(Guid courseId, IFormFile file, string instructorId)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return Result<string>.Fail("Course not found", ErrorType.NotFound);

            if (course.InstructorId != instructorId)
                return Result<string>.Fail("Access denied", ErrorType.Forbidden);

            var allowedTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedTypes.Contains(file.ContentType))
                return Result<string>.Fail("Only JPEG and PNG are allowed", ErrorType.Validation);

            const long maxSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxSize)
                return Result<string>.Fail("File size must not exceed 5 MB", ErrorType.Validation);

            var uploaded = await _fileService.UploadAsync(file, "thumbnails");
            if (!uploaded.IsSuccess || uploaded.Data == null)
                return Result<string>.Fail("Failed to upload file", ErrorType.Unknown);

            if (!string.IsNullOrEmpty(course.ThumbnailPath))
                await _fileService.DeleteAsync(course.ThumbnailPath);

            course.ThumbnailPath = uploaded.Data;
            await _courseRepository.UpdateAsync(course);

            await _cacheService.RemoveAsync("all_courses");
            await _cacheService.RemoveAsync($"course_{courseId}");

            return Result<string>.Ok(uploaded.Data);
        }
        catch (Exception ex)
        {
            return Result<string>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }
}
