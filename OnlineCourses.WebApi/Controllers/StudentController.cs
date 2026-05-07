using System;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.DTOs.Students.Request;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Constants;

namespace OnlineCourses.WebApi.Controllers;

[ApiController]
[Route("api/students")]
[ApiVersion("1.0")]
[Authorize]
public class StudentController : BaseController
{
    private readonly IStudentService _service;

    public StudentController(IStudentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await _service.GetAllStudentsAsync();

        return !users.IsSuccess ? HandleError(users) : Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id)
    {
        var user = await _service.GetStudentByIdAsync(id);

        return !user.IsSuccess ? HandleError(user) : Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] string id, UpdateStudentDto updateUserDto)
    {
        var res = await _service.UpdateStudentAsync(id, updateUserDto);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] string id)
    {
        var res = await _service.DeleteStudentAsync(id);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpPut("change-avatar/{id}")]
    public async Task<IActionResult> UploadImageAsync([FromRoute] string id, IFormFile file)
    {
        var result = await _service.UpdateStudentProfileAsync(id, file);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpDelete("delete-avatar/{id}")]
    public async Task<IActionResult> DeleteImageAsync([FromRoute] string id)
    {
        var result = await _service.DeleteStudentProfileAsync(id);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }
}
