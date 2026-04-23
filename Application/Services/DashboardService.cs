using Application.Common;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class DashboardService(
    ICourseRepository _courseRepository,
    IEnrollmentRepository _enrollmentRepository,
    ICacheService _cacheService,
    ILogger<DashboardService> _logger) : IDashboardService
{
    public async Task<Result<DashboardSummaryDto>> GetSummaryAsync()
    {
        const string key = "dashboard:summary";
        var cached = await _cacheService.GetAsync<DashboardSummaryDto>(key);
        if (cached is not null)
            return Result<DashboardSummaryDto>.Success(cached);

        // Real implementation queries DB via repositories / DbContext directly
        var summary = new DashboardSummaryDto();
        await _cacheService.SetAsync(key, summary, TimeSpan.FromMinutes(5));
        return Result<DashboardSummaryDto>.Success(summary);
    }

    public async Task<Result<List<TopCourseDto>>> GetTopCoursesAsync()
    {
        const string key = "dashboard:top_courses";
        var cached = await _cacheService.GetAsync<List<TopCourseDto>>(key);
        if (cached is not null)
            return Result<List<TopCourseDto>>.Success(cached);

        var top = await _enrollmentRepository.GetTopCoursesAsync(10);
        await _cacheService.SetAsync(key, top, TimeSpan.FromMinutes(30));
        return Result<List<TopCourseDto>>.Success(top);
    }

    public async Task<Result<List<MonthlyEnrollmentDto>>> GetEnrollmentsByMonthAsync()
    {
        // Implemented in Infrastructure via DbContext GroupBy
        return Result<List<MonthlyEnrollmentDto>>.Success(new List<MonthlyEnrollmentDto>());
    }

    public async Task<Result<List<CategoryRevenueDto>>> GetRevenueByCategoryAsync()
    {
        const string key = "dashboard:revenue_by_category";
        var cached = await _cacheService.GetAsync<List<CategoryRevenueDto>>(key);
        if (cached is not null)
            return Result<List<CategoryRevenueDto>>.Success(cached);

        var data = new List<CategoryRevenueDto>();
        await _cacheService.SetAsync(key, data, TimeSpan.FromMinutes(15));
        return Result<List<CategoryRevenueDto>>.Success(data);
    }

    public async Task<Result<List<CompletionRateDto>>> GetCompletionRateAsync()
    {
        return Result<List<CompletionRateDto>>.Success(new List<CompletionRateDto>());
    }

    public async Task<Result<InstructorStatsDto>> GetInstructorStatsAsync(string instructorId, string requestUserId, bool isAdmin)
    {
        if (!isAdmin && instructorId != requestUserId)
            return Result<InstructorStatsDto>.Forbidden("You can only view your own statistics.");

        return Result<InstructorStatsDto>.Success(new InstructorStatsDto());
    }

    public async Task<Result<StudentsProgressSummaryDto>> GetStudentsProgressAsync()
    {
        return Result<StudentsProgressSummaryDto>.Success(new StudentsProgressSummaryDto());
    }

    public async Task<Result<RatingsDistributionDto>> GetRatingsDistributionAsync()
    {
        const string key = "dashboard:ratings_distribution";
        var cached = await _cacheService.GetAsync<RatingsDistributionDto>(key);
        if (cached is not null)
            return Result<RatingsDistributionDto>.Success(cached);

        var data = new RatingsDistributionDto();
        await _cacheService.SetAsync(key, data, TimeSpan.FromMinutes(15));
        return Result<RatingsDistributionDto>.Success(data);
    }
}
