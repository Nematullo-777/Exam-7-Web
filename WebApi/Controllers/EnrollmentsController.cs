using Application.DTOs.EnrollmentDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [Authorize(Roles = UserRoles.Student)]
    [HttpPost]
    public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentDto dto)
    {
        var result = await enrollmentService.EnrollAsync(UserId, dto);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await enrollmentService.CancelEnrollmentAsync(id, UserId);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpPatch("{id:guid}/progress")]
    public async Task<IActionResult> UpdateProgress(Guid id, [FromBody] UpdateProgressDto dto)
    {
        var result = await enrollmentService.UpdateProgressAsync(id, UserId, dto);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var result = await enrollmentService.GetMyEnrollmentsAsync(UserId);
        return Ok(result.Value);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await enrollmentService.GetAllAsync(page, pageSize);
        return Ok(result.Value);
    }
}
