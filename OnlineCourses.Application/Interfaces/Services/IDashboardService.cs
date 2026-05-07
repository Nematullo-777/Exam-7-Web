using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Dashboards;

namespace OnlineCourses.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<Result<DashboardSummaryDto>> GetSummaryAsync();
    Task<Result<List<TopCourseDto>>> GetTopCoursesAsync();
    Task<Result<List<MonthlyEnrollmentDto>>> GetEnrollmentsByMonthAsync();
    Task<Result<List<CategoryRevenueDto>>> GetRevenueByCategoryAsync();
    Task<Result<List<CompletionRateDto>>> GetCompletionRateAsync();
    Task<Result<InstructorStatsDto>> GetInstructorStatsAsync(string instructorId);
    Task<Result<StudentsProgressSummaryDto>> GetStudentsProgressAsync();
    Task<Result<RatingsDistributionDto>> GetRatingsDistributionAsync();
}
