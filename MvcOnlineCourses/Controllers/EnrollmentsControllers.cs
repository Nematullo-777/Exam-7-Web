using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NewMvcApp.Models;
using NewMvcApp.Services;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Constants;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class EnrollmentsController(IEnrollmentService enrollmentService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private Guid UserGuid => Guid.Parse(UserId);
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);
    
    private readonly ApiService _api;

    public async Task<IActionResult> Index()
    {
        var result = await _api.GetMyEnrollmentsAsync();
        return View(result?.Data ?? new());
    }

    [HttpPost]
    public async Task<IActionResult> Enroll(Guid courseId)
    {
        await _api.EnrollAsync(courseId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(Guid enrollmentId)
    {
        await _api.CancelEnrollmentAsync(enrollmentId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProgress(Guid enrollmentId, int progressPercent)
    {
        await _api.UpdateProgressAsync(enrollmentId, progressPercent);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Reviews(Guid courseId)
    {
        var result = await _api.GetReviewsAsync(courseId);
        ViewBag.CourseId = courseId;
        return View(result?.Data ?? new());
    }

    [HttpGet]
    public IActionResult CreateReview(Guid courseId)
    {
        ViewBag.CourseId = courseId;
        return View(new CreateReviewDto());
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview(Guid courseId, CreateReviewDto dto)
    {
        if (!ModelState.IsValid) { ViewBag.CourseId = courseId; return View(dto); }
        var result = await _api.CreateReviewAsync(courseId, dto);
        if (result?.IsSuccess == true) return RedirectToAction("Reviews", new { courseId });
        ModelState.AddModelError("", result?.Error ?? "Ошибка");
        ViewBag.CourseId = courseId;
        return View(dto);
    }
}
