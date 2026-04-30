using Application.DTOs.EnrollmentDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[Route("api/courses/{courseId:guid}/reviews")]
[ApiController]
public class ReviewsController(IEnrollmentService enrollmentService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid courseId)
    {
        var result = await enrollmentService.GetCourseReviewsAsync(courseId);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpPost]
    public async Task<IActionResult> Create(Guid courseId, [FromBody] CreateReviewDto dto)
    {
        var result = await enrollmentService.AddReviewAsync(courseId, Guid.Parse(UserId), dto);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid courseId, Guid id, [FromBody] UpdateReviewDto dto)
    {
        var result = await enrollmentService.UpdateReviewAsync(id, Guid.Parse(UserId), dto);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid courseId, Guid id)
    {
        var result = await enrollmentService.DeleteReviewAsync(id, UserId, IsAdmin);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return NoContent();
    }
}
