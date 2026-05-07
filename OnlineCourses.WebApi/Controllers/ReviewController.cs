using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.DTOs.Reviews.Request;
using OnlineCourses.Application.Interfaces.Services;
using System.Security.Claims;
using OnlineCourses.Domain.Constants;

namespace OnlineCourses.WebApi.Controllers;

[ApiController]
[Route("api/courses/{courseId:guid}/reviews")]
[ApiVersion("1.0")]
[Authorize]
public class ReviewController : BaseController
{
    private readonly IReviewService _service;

    public ReviewController(IReviewService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid courseId)
    {
        var result = await _service.GetReviewsByCourseIdAsync(courseId);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> CreateAsync(Guid courseId, CreateReviewDto request)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.CreateReviewAsync(courseId, request, studentId);

        return !result.IsSuccess ? HandleError(result) : Created("", result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> UpdateAsync(Guid courseId, Guid id, UpdateReviewDto request)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.UpdateReviewAsync(id, request, studentId);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> DeleteAsync(Guid courseId, Guid id)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _service.DeleteReviewAsync(id, studentId);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }
}