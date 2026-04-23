using Application.Common;
using Application.DTOs.EnrollmentDTOs;
using Application.DTOs.LessonDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class EnrollmentService(
    IEnrollmentRepository _enrollmentRepository,
    ICourseRepository _courseRepository,
    ILessonRepository _lessonRepository,
    ICacheService _cacheService,
    ILogger<EnrollmentService> _logger) : IEnrollmentService
{
    public async Task<Result<EnrollmentDto>> EnrollAsync(string studentId, CreateEnrollmentDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(dto.CourseId);
        if (course is null)
            return Result<EnrollmentDto>.NotFound("Course not found.");

        if (!course.IsPublished)
            return Result<EnrollmentDto>.Failure("Course is not published.");

        var existing = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, dto.CourseId);
        if (existing is not null)
            return Result<EnrollmentDto>.Failure("You are already enrolled in this course.");

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            CourseId = dto.CourseId,
            StudentId = studentId,
            EnrolledAt = DateTime.UtcNow,
            Status = EnrollmentStatus.Active,
            ProgressPercent = 0
        };

        var created = await _enrollmentRepository.CreateAsync(enrollment);

        // Invalidate Redis cache for dashboard
        await _cacheService.RemoveAsync("dashboard:top_courses");
        await _cacheService.RemoveAsync("dashboard:summary");

        _logger.LogInformation("Student {StudentId} enrolled in course {CourseId}", studentId, dto.CourseId);
        return Result<EnrollmentDto>.Success(MapToDto(created));
    }

    public async Task<Result<bool>> CancelEnrollmentAsync(Guid enrollmentId, string studentId)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
        if (enrollment is null)
            return Result<bool>.NotFound("Enrollment not found.");

        if (enrollment.StudentId != studentId)
            return Result<bool>.Forbidden("You are not allowed to cancel this enrollment.");

        await _enrollmentRepository.DeleteAsync(enrollment);
        return Result<bool>.Success(true);
    }

    public async Task<Result<EnrollmentDto>> UpdateProgressAsync(Guid enrollmentId, string studentId, UpdateProgressDto dto)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
        if (enrollment is null)
            return Result<EnrollmentDto>.NotFound("Enrollment not found.");

        if (enrollment.StudentId != studentId)
            return Result<EnrollmentDto>.Forbidden("You are not allowed to update this enrollment.");

        enrollment.ProgressPercent = dto.ProgressPercent;
        if (dto.ProgressPercent == 100 && enrollment.Status == EnrollmentStatus.Active)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTime.UtcNow;
        }

        var updated = await _enrollmentRepository.UpdateAsync(enrollment);
        return Result<EnrollmentDto>.Success(MapToDto(updated));
    }

    public async Task<Result<List<EnrollmentDto>>> GetMyEnrollmentsAsync(string studentId)
    {
        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
        return Result<List<EnrollmentDto>>.Success(enrollments.Select(MapToDto).ToList());
    }

    public async Task<Result<PagedResult<EnrollmentDto>>> GetAllAsync(int page, int pageSize)
    {
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 50) pageSize = 50;

        var paged = await _enrollmentRepository.GetAllAsync(page, pageSize);
        return Result<PagedResult<EnrollmentDto>>.Success(new PagedResult<EnrollmentDto>
        {
            Items = paged.Items.Select(MapToDto).ToList(),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        });
    }

    public async Task<Result<ReviewDto>> AddReviewAsync(Guid courseId, string studentId, CreateReviewDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            return Result<ReviewDto>.NotFound("Course not found.");

        var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, courseId);
        if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
            return Result<ReviewDto>.Failure("You must be enrolled in the course to leave a review.");

        var alreadyReviewed = course.Reviews?.Any(r => r.StudentId == studentId) ?? false;
        if (alreadyReviewed)
            return Result<ReviewDto>.Failure("You have already reviewed this course.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            StudentId = studentId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        course.Reviews!.Add(review);
        await _courseRepository.UpdateAsync(course);

        return Result<ReviewDto>.Success(new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        });
    }

    public async Task<Result<ReviewDto>> UpdateReviewAsync(Guid reviewId, string studentId, UpdateReviewDto dto)
    {
        // Find the review across courses — simplified: search all loaded courses
        // In a real project, use a dedicated IReviewRepository
        return Result<ReviewDto>.Failure("Use a dedicated ReviewRepository for this operation.");
    }

    public async Task<Result<bool>> DeleteReviewAsync(Guid reviewId, string userId, bool isAdmin)
    {
        return Result<bool>.Failure("Use a dedicated ReviewRepository for this operation.");
    }

    public async Task<Result<List<ReviewDto>>> GetCourseReviewsAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            return Result<List<ReviewDto>>.NotFound("Course not found.");

        var reviews = course.Reviews?.Select(r => new ReviewDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            StudentName = r.Student?.FullName ?? string.Empty
        }).ToList() ?? new();

        return Result<List<ReviewDto>>.Success(reviews);
    }

    public async Task<Result<List<LessonDto>>> GetCourseLessonsAsync(Guid courseId, string userId, bool isInstructor, bool isAdmin)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            return Result<List<LessonDto>>.NotFound("Course not found.");

        if (!isAdmin && !(isInstructor && course.InstructorId == userId))
        {
            var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(userId, courseId);
            if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
                return Result<List<LessonDto>>.Forbidden("You must be enrolled in this course to view lessons.");
        }

        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        var dtos = lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            Title = l.Title,
            Content = l.Content,
            VideoUrl = l.VideoUrl,
            Order = l.Order,
            DurationMinutes = l.DurationMinutes,
            CourseId = l.CourseId
        }).ToList();

        return Result<List<LessonDto>>.Success(dtos);
    }

    private static EnrollmentDto MapToDto(Enrollment e) => new()
    {
        Id = e.Id,
        EnrolledAt = e.EnrolledAt,
        CompletedAt = e.CompletedAt,
        Status = e.Status,
        ProgressPercent = e.ProgressPercent,
        CourseId = e.CourseId,
        CourseTitle = e.Course?.Title ?? string.Empty,
        StudentId = e.StudentId,
        StudentName = e.Student?.FullName ?? string.Empty
    };
}
