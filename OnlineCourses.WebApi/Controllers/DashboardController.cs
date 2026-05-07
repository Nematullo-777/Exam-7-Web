using System;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.Interfaces.Services;

namespace OnlineCourses.WebApi.Controllers;

[ApiController]
[Route("api/dashboards")]
[ApiVersion("1.0")]
[Authorize(Roles = "Admin")]
public class DashboardController : BaseController
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummaryAsync()
    {
        var result = await _service.GetSummaryAsync();

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpGet("top-courses")]
    public async Task<IActionResult> GetTopCoursesAsync()
    {
        var result = await _service.GetTopCoursesAsync();

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpGet("enrollments-by-month")]
    public async Task<IActionResult> GetEnrollmentsByMonthAsync()
    {
        var result = await _service.GetEnrollmentsByMonthAsync();

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }
}
