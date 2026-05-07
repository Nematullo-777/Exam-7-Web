using System;
using OnlineCourses.Application.DTOs.Dashboards;

namespace OnlineCourses.Application.Interfaces.Repositories;

public interface IDashboardSummaryRepository
{
    Task<int> GetTotalCoursesAsync();
    Task<int> GetPublishedCoursesAsync();
    Task<int> GetTotalStudentsAsync();
    Task<int> GetTotalInstructorsAsync();
    Task<int> GetTotalEnrollmentsAsync();
    Task<int> GetActiveEnrollmentsAsync();
    Task<int> GetCompletedEnrollmentsAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<int> GetTotalReviewsAsync();
    Task<double> GetAverageRatingAsync();
    Task<List<TopCourseDto>> GetTopCoursesAsync(int count);
    Task<List<MonthlyEnrollmentDto>> GetEnrollmentsByMonthAsync();
    Task<List<CategoryRevenueDto>> GetRevenueByCategoryAsync();
    Task<List<CompletionRateDto>> GetCompletionRateAsync();
    Task<InstructorStatsDto> GetInstructorStatsAsync(string instructorId);
    Task<StudentsProgressSummaryDto> GetStudentsProgressAsync();
    Task<RatingsDistributionDto> GetRatingsDistributionAsync();
}
