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
        return View(result.Value?.Items ?? new List<Application.DTOs.StudentDTOs.StudentDto>());
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
