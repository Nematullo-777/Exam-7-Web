using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.DTOs.Enrollments.Request;
using OnlineCourses.Application.Interfaces.Services;
using System.Security.Claims;
using OnlineCourses.Domain.Constants;

namespace OnlineCourses.WebApi.Controllers;

[ApiController]
[Route("api/enrollments")]
[ApiVersion("1.0")]
[Authorize]
public class EnrollmentController : BaseController
{
    private readonly IEnrollmentService _service;

    public EnrollmentController(IEnrollmentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _service.GetAllEnrollmentsAsync();

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpGet("my")]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> GetMyAsync()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.GetMyEnrollmentsAsync(studentId);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> CreateAsync(CreateEnrollmentDto request)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.CreateEnrollmentAsync(request, studentId);

        return !result.IsSuccess ? HandleError(result) : Created("", result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.DeleteEnrollmentAsync(id, studentId);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpPatch("{id:guid}/progress")]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> UpdateProgressAsync(Guid id, UpdateEnrollmentDto request)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.UpdateProgressAsync(id, request, studentId);
        
        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }
}