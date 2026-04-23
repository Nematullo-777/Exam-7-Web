using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = UserRoles.Admin)]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await dashboardService.GetSummaryAsync();
        return Ok(result.Value);
    }

    [HttpGet("top-courses")]
    public async Task<IActionResult> GetTopCourses()
    {
        var result = await dashboardService.GetTopCoursesAsync();
        return Ok(result.Value);
    }

    [HttpGet("enrollments-by-month")]
    public async Task<IActionResult> GetEnrollmentsByMonth()
    {
        var result = await dashboardService.GetEnrollmentsByMonthAsync();
        return Ok(result.Value);
    }

    [HttpGet("revenue-by-category")]
    public async Task<IActionResult> GetRevenueByCategory()
    {
        var result = await dashboardService.GetRevenueByCategoryAsync();
        return Ok(result.Value);
    }

    [HttpGet("completion-rate")]
    public async Task<IActionResult> GetCompletionRate()
    {
        var result = await dashboardService.GetCompletionRateAsync();
        return Ok(result.Value);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Instructor}")]
    [HttpGet("instructor/{instructorId}")]
    public async Task<IActionResult> GetInstructorStats(string instructorId)
    {
        var result = await dashboardService.GetInstructorStatsAsync(instructorId, UserId, IsAdmin);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("students-progress")]
    public async Task<IActionResult> GetStudentsProgress()
    {
        var result = await dashboardService.GetStudentsProgressAsync();
        return Ok(result.Value);
    }

    [HttpGet("ratings-distribution")]
    public async Task<IActionResult> GetRatingsDistribution()
    {
        var result = await dashboardService.GetRatingsDistributionAsync();
        return Ok(result.Value);
    }
}
