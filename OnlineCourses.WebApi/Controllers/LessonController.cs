using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.DTOs.Lessons.Request;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Constants;

namespace OnlineCourses.WebApi.Controllers;

[ApiController]
[Route("api/courses/{courseId:guid}/lessons")]
[ApiVersion("1.0")]
[Authorize]
public class LessonController : BaseController
{
    private readonly ILessonService _service;

    public LessonController(ILessonService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid courseId)
    {
        var result = await _service.GetLessonsByCourseIdAsync(courseId);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid courseId, Guid id)
    {
        var result = await _service.GetLessonByIdAsync(id);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> CreateAsync(Guid courseId, CreateLessonDto request)
    {
        var result = await _service.CreateLessonAsync(courseId, request);

        return !result.IsSuccess ? HandleError(result) : Created("", result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> UpdateAsync(Guid courseId, Guid id, UpdateLessonDto request)
    {
        var result = await _service.UpdateLessonAsync(id, request);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> DeleteAsync(Guid courseId, Guid id)
    {
        var result = await _service.DeleteLessonAsync(id);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }
}