using Application.DTOs.CategoryDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcOnlineCourses.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class CategoriesController(ICategoryService categoryService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await categoryService.GetAllAsync();
        return View(result.Value ?? new List<CategoryDto>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateCategoryDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await categoryService.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка при создании категории");
            return View(dto);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await categoryService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var cat = result.Value!;
        ViewBag.Id = id;
        return View(new UpdateCategoryDto
        {
            Name = cat.Name,
            Description = cat.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Id = id;
            return View(dto);
        }

        var result = await categoryService.UpdateAsync(id, dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка при обновлении");
            ViewBag.Id = id;
            return View(dto);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await categoryService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}