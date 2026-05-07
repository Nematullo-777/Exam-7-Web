using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NewMvcApp.Models;
using NewMvcApp.Services;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Constants;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class StudentsController(IStudentService studentService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);

    private readonly ApiService _api;

    public async Task<IActionResult> Index()
    {
        var result = await _api.GetStudentsAsync();
        return View(result?.Data ?? new());
    }

    public async Task<IActionResult> Details(string id)
    {
        var result = await _api.GetStudentAsync(id);
        if (result?.IsSuccess != true) return NotFound();
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var result = await _api.GetStudentAsync(id);
        if (result?.IsSuccess != true) return NotFound();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, UpdateStudentDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var result = await _api.UpdateStudentAsync(id, dto);
        if (result?.IsSuccess == true) return RedirectToAction("Details", new { id });
        ModelState.AddModelError("", result?.Error ?? "Ошибка");
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        await _api.DeleteStudentAsync(id);
        return RedirectToAction("Index");
    }
}