using Application.Common;
using Application.DTOs.LessonDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class LessonService(
    ILessonRepository lessonRepository,
    ICourseRepository courseRepository,
    IFileService fileService) : ILessonService
{
    public async Task<Result<List<LessonDto>>> GetByCourseIdAsync(Guid courseId)
    {
        var lessons = await lessonRepository.GetByCourseIdAsync(courseId);
        var dtos = lessons.Select(MapToDto).ToList();
        return Result<List<LessonDto>>.Success(dtos);
    }

    public async Task<Result<LessonDto>> GetByIdAsync(Guid id)
    {
        var lesson = await lessonRepository.GetByIdAsync(id);
        if (lesson is null)
            return Result<LessonDto>.NotFound("Lesson not found.");

        return Result<LessonDto>.Success(MapToDto(lesson));
    }

    public async Task<Result<LessonDto>> CreateAsync(Guid courseId, string instructorId, CreateLessonDto dto)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course is null)
            return Result<LessonDto>.NotFound("Course not found.");

        if (course.InstructorId != instructorId)
            return Result<LessonDto>.Forbidden("You are not the instructor of this course.");

        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Content,
            VideoUrl = dto.VideoUrl,
            Order = dto.Order,
            CourseId = courseId,
            CreatedAt = DateTime.UtcNow,
            MaterialPath = string.Empty
        };

        var created = await lessonRepository.CreateAsync(lesson);
        return Result<LessonDto>.Success(MapToDto(created));
    }

    public async Task<Result<LessonDto>> UpdateAsync(Guid id, string instructorId, bool isAdmin, UpdateLessonDto dto)
    {
        var lesson = await lessonRepository.GetByIdAsync(id);
        if (lesson is null)
            return Result<LessonDto>.NotFound("Lesson not found.");

        if (!isAdmin && lesson.Course.InstructorId != instructorId)
            return Result<LessonDto>.Forbidden("You are not the instructor of this course.");

        lesson.Title = dto.Title;
        lesson.Description = dto.Content;
        lesson.VideoUrl = dto.VideoUrl;
        lesson.Order = dto.Order;

        var updated = await lessonRepository.UpdateAsync(lesson);
        return Result<LessonDto>.Success(MapToDto(updated));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, string instructorId, bool isAdmin)
    {
        var lesson = await lessonRepository.GetByIdAsync(id);
        if (lesson is null)
            return Result<bool>.NotFound("Lesson not found.");

        if (!isAdmin && lesson.Course.InstructorId != instructorId)
            return Result<bool>.Forbidden("You are not the instructor of this course.");

        if (!string.IsNullOrEmpty(lesson.MaterialPath))
            await fileService.DeleteAsync(lesson.MaterialPath);

        await lessonRepository.DeleteAsync(lesson);
        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> UploadMaterialAsync(Guid id, string instructorId, IFormFile file)
    {
        var lesson = await lessonRepository.GetByIdAsync(id);
        if (lesson is null)
            return Result<string>.NotFound("Lesson not found.");

        if (lesson.Course.InstructorId != instructorId)
            return Result<string>.Forbidden("You are not the instructor of this course.");

        if (!string.IsNullOrEmpty(lesson.MaterialPath))
            await fileService.DeleteAsync(lesson.MaterialPath);

        var path = await fileService.UploadAsync(file, "materials");
        if (path is null)
            return Result<string>.Failure("Failed to upload material.");

        lesson.MaterialPath = path;
        await lessonRepository.UpdateAsync(lesson);

        return Result<string>.Success(path);
    }

    private static LessonDto MapToDto(Lesson l) => new()
    {
        Id = l.Id,
        Title = l.Title,
        Content = l.Description ?? string.Empty,
        VideoUrl = l.VideoUrl,
        Order = l.Order,
        CourseId = l.CourseId
    };
}
