using Application.DTOs.StudentDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class StudentsController(IStudentService studentService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await studentService.GetAllAsync(page, 20);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result.Value?.TotalPages ?? 1;
        return View(result.Value?.Items ?? new List<StudentDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var result = await studentService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        if (!IsAdmin && id != UserId)
            return Forbid();

        return View(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (!IsAdmin && id != UserId)
            return Forbid();

        var result = await studentService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var student = result.Value!;
        ViewBag.StudentId = id;
        return View(new UpdateStudentDto
        {
            FullName = student.FullName,
            Bio = student.Bio,
            AvatarUrl = student.AvatarUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UpdateStudentDto dto)
    {
        if (!IsAdmin && id != UserId)
            return Forbid();

        if (!ModelState.IsValid)
        {
            ViewBag.StudentId = id;
            return View(dto);
        }

        var result = await studentService.UpdateAsync(id, UserId, IsAdmin, dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка при обновлении");
            ViewBag.StudentId = id;
            return View(dto);
        }

        return RedirectToAction(IsAdmin ? "Index" : "Details", new { id });
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        await studentService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}