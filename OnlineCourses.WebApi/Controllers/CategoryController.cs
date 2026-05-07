using System;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.DTOs.Categories.Request;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Constants;

namespace OnlineCourses.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class CategoryController : BaseController
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAsync()
    {
        var categories = await _service.GetAllCategoriesAsync();

        return !categories.IsSuccess ? HandleError(categories) : Ok(categories);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var categories = await _service.GetCategoryByIdAsync(id);

        return !categories.IsSuccess ? HandleError(categories) : Ok(categories);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> CreateAsync(CreateCategoryDto request)
    {
        var categories = await _service.CreateCategoryAsync(request);

        return !categories.IsSuccess ? HandleError(categories) : Created("", categories);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateCategoryDto request)
    {
        var categories = await _service.UpdateCategoryAsync(id, request);

        return !categories.IsSuccess ? HandleError(categories) : Ok(categories);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var categories = await _service.DeleteCategoryAsync(id);

        return !categories.IsSuccess ? HandleError(categories) : Ok(categories);
    }
}
