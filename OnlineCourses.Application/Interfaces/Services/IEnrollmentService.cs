using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Enrollments.Request;
using OnlineCourses.Application.DTOs.Enrollments.Response;

namespace OnlineCourses.Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<Result<List<GetEnrollmentDto>>> GetAllEnrollmentsAsync();
    Task<Result<List<GetEnrollmentDto>>> GetMyEnrollmentsAsync(string studentId);
    Task<Result<CreateEnrollmentResponseDto>> CreateEnrollmentAsync(CreateEnrollmentDto request, string studentId);
    Task<Result<DeleteEnrollmentResponseDto>> DeleteEnrollmentAsync(Guid id, string studentId);
    Task<Result<GetEnrollmentDto>> UpdateProgressAsync(Guid id, UpdateEnrollmentDto request, string studentId);
}
