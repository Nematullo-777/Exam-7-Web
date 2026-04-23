using Application.Common;
using Application.DTOs.CourseDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CourseService(
    ICourseRepository courseRepository,
    IFileService fileService,
    ILogger<CourseService> logger) : ICourseService
{
    public async Task<Result<PagedResult<CourseDto>>> GetAllAsync(CourseFilterDto filter)
    {
        if (filter.PageSize < 1) filter.PageSize = 1;
        if (filter.PageSize > 50) filter.PageSize = 50;

        var pagedCourses = await courseRepository.GetAllAsync(filter);
        var dtos = pagedCourses.Items.Select(MapToDto).ToList();

        return Result<PagedResult<CourseDto>>.Success(new PagedResult<CourseDto>
        {
            Items = dtos,
            TotalCount = pagedCourses.TotalCount,
            Page = pagedCourses.Page,
            PageSize = pagedCourses.PageSize
        });
    }

    public async Task<Result<CourseDto>> GetByIdAsync(Guid id)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course is null)
            return Result<CourseDto>.NotFound("Course not found.");

        return Result<CourseDto>.Success(MapToDto(course));
    }

    public async Task<Result<CourseDto>> CreateAsync(string instructorId, CreateCourseDto dto)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Level = dto.Level,
            CategoryId = dto.CategoryId,
            InstructorId = instructorId,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await courseRepository.CreateAsync(course);
        logger.LogInformation("Course {CourseId} created by instructor {InstructorId}", created.Id, instructorId);

        return Result<CourseDto>.Success(MapToDto(created));
    }

    public async Task<Result<CourseDto>> UpdateAsync(Guid id, string instructorId, UpdateCourseDto dto)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course is null)
            return Result<CourseDto>.NotFound("Course not found.");

        if (course.InstructorId != instructorId)
            return Result<CourseDto>.Forbidden("You are not allowed to update this course.");

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Price = dto.Price;
        course.Level = dto.Level;
        course.CategoryId = dto.CategoryId;

        var updated = await courseRepository.UpdateAsync(course);
        return Result<CourseDto>.Success(MapToDto(updated));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, string userId, bool isAdmin)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course is null)
            return Result<bool>.NotFound("Course not found.");

        if (!isAdmin && course.InstructorId != userId)
            return Result<bool>.Forbidden("You are not allowed to delete this course.");

        await courseRepository.DeleteAsync(course);
        logger.LogInformation("Course {CourseId} deleted by user {UserId}", id, userId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> TogglePublishAsync(Guid id, string instructorId)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course is null)
            return Result<bool>.NotFound("Course not found.");

        if (course.InstructorId != instructorId)
            return Result<bool>.Forbidden("You are not allowed to publish this course.");

        course.IsPublished = !course.IsPublished;
        await courseRepository.UpdateAsync(course);
        return Result<bool>.Success(course.IsPublished);
    }

    public async Task<Result<string>> UploadThumbnailAsync(Guid id, string instructorId, IFormFile file)
    {
        var course = await courseRepository.GetByIdAsync(id);
        if (course is null)
            return Result<string>.NotFound("Course not found.");

        if (course.InstructorId != instructorId)
            return Result<string>.Forbidden("You are not allowed to modify this course.");

        var allowedTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedTypes.Contains(file.ContentType))
            return Result<string>.Failure("Only JPEG and PNG images are allowed.");

        const long maxSize = 5 * 1024 * 1024; // 5MB
        if (file.Length > maxSize)
            return Result<string>.Failure("File size must not exceed 5 MB.");

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{ext}";

        var filePath = await fileService.UploadAsync(file, "thumbnails");
        if (filePath is null)
            return Result<string>.Failure("File upload failed.");

        if (!string.IsNullOrEmpty(course.ThumbnailPath))
            await fileService.DeleteAsync(course.ThumbnailPath);

        course.ThumbnailPath = filePath;
        await courseRepository.UpdateAsync(course);

        return Result<string>.Success(filePath);
    }

    private static CourseDto MapToDto(Course c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        ThumbnailPath = c.ThumbnailPath,
        Price = c.Price,
        Level = c.Level,
        IsPublished = c.IsPublished,
        CreatedAt = c.CreatedAt,
        CategoryId = c.CategoryId,
        CategoryName = c.Category?.Name ?? string.Empty,
        InstructorId = c.InstructorId,
        InstructorName = c.Instructor?.FullName ?? string.Empty,
        LessonCount = c.Lessons?.Count ?? 0,
        EnrollmentCount = c.Enrollments?.Count ?? 0,
        AverageRating = c.Reviews?.Any() == true ? c.Reviews.Average(r => r.Rating) : 0
    };
}
