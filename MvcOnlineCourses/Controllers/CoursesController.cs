using Application.DTOs.CourseDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcOnlineCourses.ViewModels;
using System.Security.Claims;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class CoursesController(
    ICourseService courseService,
    ICategoryService categoryService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);
    private bool IsInstructor => User.IsInRole(UserRoles.Instructor);

    [HttpGet]
    public async Task<IActionResult> Index(string? search, Guid? categoryId, bool? isPublished)
    {
        var filter = new CourseFilterDto
        {
            Search = search,
            CategoryId = categoryId,
            IsPublished = isPublished,
            PageSize = 50
        };

        var result = await courseService.GetAllAsync(filter);
        var categories = await categoryService.GetAllAsync();

        ViewBag.Categories = categories.Value ?? new List<Application.DTOs.CategoryDTOs.CategoryDto>();
        ViewBag.Search = search;
        ViewBag.SelectedCategory = categoryId;
        ViewBag.IsPublished = isPublished;

        return View(result.Value?.Items ?? new List<CourseDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await courseService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();
        return View(result.Value);
    }

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCourseDto dto, IFormFile? thumbnail)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index");

        var result = await courseService.CreateAsync(UserId, dto);

        if (result.IsSuccess && thumbnail != null)
            await courseService.UploadThumbnailAsync(result.Value!.Id, UserId, thumbnail);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await courseService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var course = result.Value!;

        if (!IsAdmin && course.InstructorId != UserId)
            return Forbid();

        var categories = await categoryService.GetAllAsync();
        ViewBag.Categories = categories.Value ?? new List<Application.DTOs.CategoryDTOs.CategoryDto>();

        return View(new EditCourseViewModel
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Price = course.Price,
            Level = course.Level,
            CategoryId = course.CategoryId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCourseViewModel vm, IFormFile? thumbnail)
    {
        if (!ModelState.IsValid)
        {
            var cats = await categoryService.GetAllAsync();
            ViewBag.Categories = cats.Value ?? new List<Application.DTOs.CategoryDTOs.CategoryDto>();
            return View(vm);
        }

        var result = await courseService.UpdateAsync(vm.Id, UserId, new UpdateCourseDto
        {
            Title = vm.Title,
            Description = vm.Description,
            Price = vm.Price,
            Level = vm.Level,
            CategoryId = vm.CategoryId
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка");
            var cats = await categoryService.GetAllAsync();
            ViewBag.Categories = cats.Value ?? new List<Application.DTOs.CategoryDTOs.CategoryDto>();
            return View(vm);
        }

        if (thumbnail != null)
            await courseService.UploadThumbnailAsync(vm.Id, UserId, thumbnail);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await courseService.DeleteAsync(id, UserId, IsAdmin);
        return RedirectToAction("Index");
    }

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(Guid id)
    {
        await courseService.TogglePublishAsync(id, UserId);
        return RedirectToAction("Index");
    }
}
