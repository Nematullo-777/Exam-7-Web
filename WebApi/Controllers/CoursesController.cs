using Application.DTOs.CourseDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CourseFilterDto filter)
    {
        var result = await courseService.GetAllAsync(filter);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await courseService.GetByIdAsync(id);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
    {
        var result = await courseService.CreateAsync(UserId, dto);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto dto)
    {
        var result = await courseService.UpdateAsync(id, UserId, dto);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await courseService.DeleteAsync(id, UserId, IsAdmin);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> TogglePublish(Guid id)
    {
        var result = await courseService.TogglePublishAsync(id, UserId);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(new { isPublished = result.Value });
    }

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPost("{id:guid}/thumbnail")]
    public async Task<IActionResult> UploadThumbnail(Guid id, IFormFile file)
    {
        var result = await courseService.UploadThumbnailAsync(id, UserId, file);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(new { path = result.Value });
    }
}
