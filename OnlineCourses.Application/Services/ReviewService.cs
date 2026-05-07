using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Reviews.Request;
using OnlineCourses.Application.DTOs.Reviews.Response;
using OnlineCourses.Application.Interfaces.Repositories;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Entities;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICacheService _cacheService;

    public ReviewService(
        IReviewRepository reviewRepository,
        IEnrollmentRepository enrollmentRepository,
        ICacheService cacheService)
    {
        _reviewRepository = reviewRepository;
        _enrollmentRepository = enrollmentRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<List<GetReviewDto>>> GetReviewsByCourseIdAsync(Guid courseId)
    {
        var cacheKey = $"reviews_{courseId}";
        var cached = await _cacheService.GetAsync<List<GetReviewDto>>(cacheKey);
        if (cached != null)
            return Result<List<GetReviewDto>>.Ok(cached);

        var reviews = await _reviewRepository.GetByCourseIdAsync(courseId);

        var result = reviews.Select(r => new GetReviewDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CraetedAt,
            CourseId = r.CourseId,
            CourseTitle = r.Course.Title,
            StudentId = r.StudentId,
            StudentName = r.Student.FullName
        }).ToList();

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

        return Result<List<GetReviewDto>>.Ok(result);
    }

    public async Task<Result<CreateReviewResponseDto>> CreateReviewAsync(
        Guid courseId, CreateReviewDto request, string studentId)
    {
        try
        {
            var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, courseId);
            if (enrollment == null)
                return Result<CreateReviewResponseDto>.Fail("You are not enrolled in this course", ErrorType.Forbidden);

            var existing = await _reviewRepository.GetByStudentAndCourseAsync(studentId, courseId);
            if (existing != null)
                return Result<CreateReviewResponseDto>.Fail("You have already reviewed this course", ErrorType.Validation);

            if (request.Rating < 1 || request.Rating > 5)
                return Result<CreateReviewResponseDto>.Fail("Rating must be between 1 and 5", ErrorType.Validation);

            var review = new Review
            {
                Rating = request.Rating,
                Comment = request.Comment,
                CourseId = courseId,
                StudentId = studentId,
                CraetedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
            await _cacheService.RemoveAsync($"reviews_{courseId}");
            await _cacheService.RemoveAsync($"course_{courseId}");

            return Result<CreateReviewResponseDto>.Ok(new CreateReviewResponseDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CraetedAt,
                CourseId = review.CourseId,
                StudentId = review.StudentId
            });
        }
        catch (Exception ex)
        {
            return Result<CreateReviewResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<UpdateReviewResponseDto>> UpdateReviewAsync(
        Guid id, UpdateReviewDto request, string studentId)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return Result<UpdateReviewResponseDto>.Fail("Review not found", ErrorType.NotFound);

            if (review.StudentId != studentId)
                return Result<UpdateReviewResponseDto>.Fail("Access denied", ErrorType.Forbidden);

            if (request.Rating < 1 || request.Rating > 5)
                return Result<UpdateReviewResponseDto>.Fail("Rating must be between 1 and 5", ErrorType.Validation);

            review.Rating = request.Rating;
            review.Comment = request.Comment;

            await _reviewRepository.UpdateAsync(review);
            await _cacheService.RemoveAsync($"reviews_{review.CourseId}");
            await _cacheService.RemoveAsync($"course_{review.CourseId}");

            return Result<UpdateReviewResponseDto>.Ok(new UpdateReviewResponseDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CourseId = review.CourseId
            });
        }
        catch (Exception ex)
        {
            return Result<UpdateReviewResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<DeleteReviewResponseDto>> DeleteReviewAsync(
        Guid id, string studentId)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return Result<DeleteReviewResponseDto>.Fail("Review not found", ErrorType.NotFound);
           
            await _reviewRepository.DeleteAsync(review);
            await _cacheService.RemoveAsync($"reviews_{review.CourseId}");
            await _cacheService.RemoveAsync($"course_{review.CourseId}");

            return Result<DeleteReviewResponseDto>.Ok(new DeleteReviewResponseDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment
            });
        }
        catch (Exception ex)
        {
            return Result<DeleteReviewResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }
}
