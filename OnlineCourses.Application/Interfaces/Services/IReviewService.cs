using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Reviews.Request;
using OnlineCourses.Application.DTOs.Reviews.Response;

namespace OnlineCourses.Application.Interfaces.Services;

public interface IReviewService
{
    Task<Result<List<GetReviewDto>>> GetReviewsByCourseIdAsync(Guid courseId);
    Task<Result<CreateReviewResponseDto>> CreateReviewAsync(Guid courseId, CreateReviewDto request, string studentId);
    Task<Result<UpdateReviewResponseDto>> UpdateReviewAsync(Guid id, UpdateReviewDto request, string studentId);
    Task<Result<DeleteReviewResponseDto>> DeleteReviewAsync(Guid id, string studentId);
}
