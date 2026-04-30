using Application.DTOs.EnrollmentDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class EnrollmentsController(IEnrollmentService enrollmentService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private Guid UserGuid => Guid.Parse(UserId);
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);
    
    [HttpGet]
    public async Task<IActionResult> MyEnrollments()
    {
        var result = await enrollmentService.GetMyEnrollmentsAsync(UserId);
        return View(result.Value ?? new List<EnrollmentDto>());
    }
    
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var result = await enrollmentService.GetAllAsync(page, 20);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = result.Value?.TotalPages ?? 1;
        return View(result.Value?.Items ?? new List<EnrollmentDto>());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(Guid courseId)
    {
        var result = await enrollmentService.EnrollAsync(UserId, new CreateEnrollmentDto
        {
            CourseId = courseId
        });

        if (!result.IsSuccess)
            TempData["Error"] = result.Error;

        return RedirectToAction("MyEnrollments");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid enrollmentId)
    {
        await enrollmentService.CancelEnrollmentAsync(enrollmentId, UserGuid);
        return RedirectToAction("MyEnrollments");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProgress(Guid enrollmentId, int progressPercent)
    {
        await enrollmentService.UpdateProgressAsync(enrollmentId, UserId, new UpdateProgressDto
        {
            ProgressPercent = progressPercent
        });
        return RedirectToAction("MyEnrollments");
    }
    
    [HttpGet]
    public async Task<IActionResult> Reviews(Guid courseId)
    {
        var result = await enrollmentService.GetCourseReviewsAsync(courseId);
        ViewBag.CourseId = courseId;
        return View(result.Value ?? new List<ReviewDto>());
    }

    [HttpGet]
    public IActionResult CreateReview(Guid courseId)
    {
        ViewBag.CourseId = courseId;
        return View(new CreateReviewDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateReview(Guid courseId, CreateReviewDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CourseId = courseId;
            return View(dto);
        }

        var result = await enrollmentService.AddReviewAsync(courseId, UserGuid, dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка при добавлении отзыва");
            ViewBag.CourseId = courseId;
            return View(dto);
        }

        return RedirectToAction("Reviews", new { courseId });
    }

    [HttpGet]
    public IActionResult EditReview(Guid reviewId, Guid courseId)
    {
        ViewBag.ReviewId = reviewId;
        ViewBag.CourseId = courseId;
        return View(new UpdateReviewDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditReview(Guid reviewId, Guid courseId, UpdateReviewDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReviewId = reviewId;
            ViewBag.CourseId = courseId;
            return View(dto);
        }

        var result = await enrollmentService.UpdateReviewAsync(reviewId, UserGuid, dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка при обновлении отзыва");
            ViewBag.ReviewId = reviewId;
            ViewBag.CourseId = courseId;
            return View(dto);
        }

        return RedirectToAction("Reviews", new { courseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(Guid reviewId, Guid courseId)
    {
        await enrollmentService.DeleteReviewAsync(reviewId, UserId, IsAdmin);
        return RedirectToAction("Reviews", new { courseId });
    }
}
