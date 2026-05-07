using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Dashboards;
using OnlineCourses.Application.Interfaces.Repositories;
using OnlineCourses.Application.Interfaces.Services;

namespace OnlineCourses.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardSummaryRepository _dashboardRepository;
    private readonly ICacheService _cacheService;

    public DashboardService(IDashboardSummaryRepository dashboardRepository, ICacheService cacheService)
    {
        _dashboardRepository = dashboardRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<DashboardSummaryDto>> GetSummaryAsync()
    {
        const string cacheKey = "dashboard:summary";
        var cached = await _cacheService.GetAsync<DashboardSummaryDto>(cacheKey);
        if (cached != null)
            return Result<DashboardSummaryDto>.Ok(cached);

        var result = new DashboardSummaryDto
        {
            TotalCourses = await _dashboardRepository.GetTotalCoursesAsync(),
            PublishedCourses = await _dashboardRepository.GetPublishedCoursesAsync(),
            TotalStudents = await _dashboardRepository.GetTotalStudentsAsync(),
            TotalInstructors = await _dashboardRepository.GetTotalInstructorsAsync(),
            TotalEnrollments = await _dashboardRepository.GetTotalEnrollmentsAsync(),
            ActiveEnrollments = await _dashboardRepository.GetActiveEnrollmentsAsync(),
            CompletedEnrollments = await _dashboardRepository.GetCompletedEnrollmentsAsync(),
            TotalRevenue = await _dashboardRepository.GetTotalRevenueAsync(),
            TotalReviews = await _dashboardRepository.GetTotalReviewsAsync(),
            AveragePlatformRating = await _dashboardRepository.GetAverageRatingAsync()
        };

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return Result<DashboardSummaryDto>.Ok(result);
    }

    public async Task<Result<List<TopCourseDto>>> GetTopCoursesAsync()
    {
        const string cacheKey = "dashboard:top_courses";
        var cached = await _cacheService.GetAsync<List<TopCourseDto>>(cacheKey);
        if (cached != null)
            return Result<List<TopCourseDto>>.Ok(cached);

        var result = await _dashboardRepository.GetTopCoursesAsync(10);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

        return Result<List<TopCourseDto>>.Ok(result);
    }

    public async Task<Result<List<MonthlyEnrollmentDto>>> GetEnrollmentsByMonthAsync()
    {
        var result = await _dashboardRepository.GetEnrollmentsByMonthAsync();

        return Result<List<MonthlyEnrollmentDto>>.Ok(result);
    }
    
    public async Task<Result<List<CategoryRevenueDto>>> GetRevenueByCategoryAsync()
    {
        const string cacheKey = "dashboard:revenue_by_category";
        var cached = await _cacheService.GetAsync<List<CategoryRevenueDto>>(cacheKey);
        if (cached != null) return Result<List<CategoryRevenueDto>>.Ok(cached);

        var result = await _dashboardRepository.GetRevenueByCategoryAsync();
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));
        return Result<List<CategoryRevenueDto>>.Ok(result);
    }

    public async Task<Result<List<CompletionRateDto>>> GetCompletionRateAsync()
    {
        var result = await _dashboardRepository.GetCompletionRateAsync();
        return Result<List<CompletionRateDto>>.Ok(result);
    }

    public async Task<Result<InstructorStatsDto>> GetInstructorStatsAsync(string instructorId)
    {
        var result = await _dashboardRepository.GetInstructorStatsAsync(instructorId);
        return Result<InstructorStatsDto>.Ok(result);
    }

    public async Task<Result<StudentsProgressSummaryDto>> GetStudentsProgressAsync()
    {
        var result = await _dashboardRepository.GetStudentsProgressAsync();
        return Result<StudentsProgressSummaryDto>.Ok(result);
    }

    public async Task<Result<RatingsDistributionDto>> GetRatingsDistributionAsync()
    {
        const string cacheKey = "dashboard:ratings_distribution";
        var cached = await _cacheService.GetAsync<RatingsDistributionDto>(cacheKey);
        if (cached != null) return Result<RatingsDistributionDto>.Ok(cached);

        var result = await _dashboardRepository.GetRatingsDistributionAsync();
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));
        return Result<RatingsDistributionDto>.Ok(result);
    }
}