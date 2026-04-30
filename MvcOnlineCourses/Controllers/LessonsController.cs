using Application.DTOs.LessonDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcOnlineCourses.ViewModels;
using System.Security.Claims;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class LessonsController(
    ILessonService lessonService,
    ICourseService courseService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);
    private bool IsInstructor => User.IsInRole(UserRoles.Instructor);

    [HttpGet]
    public async Task<IActionResult> Index(Guid courseId)
    {
        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess) return NotFound();

        var lessons = await lessonService.GetByCourseIdAsync(courseId);
        ViewBag.Course = course.Value;
        return View(lessons.Value ?? new List<LessonDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, Guid courseId)
    {
        var result = await lessonService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();
        ViewBag.CourseId = courseId;
        return View(result.Value);
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Instructor}")]
    [HttpGet]
    public async Task<IActionResult> Create(Guid courseId)
    {
        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess) return NotFound();
        ViewBag.Course = course.Value;
        return View(new CreateLessonViewModel { CourseId = courseId });
    }

    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Instructor}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLessonViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var course = await courseService.GetByIdAsync(vm.CourseId);
            ViewBag.Course = course.Value;
            return View(vm);
        }

        var result = await lessonService.CreateAsync(vm.CourseId, UserId, new CreateLessonDto
        {
            Title = vm.Title,
            Content = vm.Content,
            VideoUrl = vm.VideoUrl,
            Order = vm.Order,
            DurationMinutes = vm.DurationMinutes
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка при создании урока");
            var course = await courseService.GetByIdAsync(vm.CourseId);
            ViewBag.Course = course.Value;
            return View(vm);
        }

        return RedirectToAction("Index", new { courseId = vm.CourseId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, Guid courseId)
    {
        var result = await lessonService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var lesson = result.Value!;

        var course = await courseService.GetByIdAsync(courseId);
        if (!course.IsSuccess) return NotFound();

        if (!IsAdmin && course.Value!.InstructorId != UserId)
            return Forbid();

        ViewBag.Course = course.Value;

        return View(new EditLessonViewModel
        {
            Id = lesson.Id,
            CourseId = courseId,
            Title = lesson.Title,
            Content = lesson.Content,
            VideoUrl = lesson.VideoUrl,
            Order = lesson.Order,
            DurationMinutes = lesson.DurationMinutes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditLessonViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var course = await courseService.GetByIdAsync(vm.CourseId);
            ViewBag.Course = course.Value;
            return View(vm);
        }

        var result = await lessonService.UpdateAsync(vm.Id, UserId, IsAdmin, new UpdateLessonDto
        {
            Title = vm.Title,
            Content = vm.Content,
            VideoUrl = vm.VideoUrl,
            Order = vm.Order,
            DurationMinutes = vm.DurationMinutes
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка при обновлении урока");
            var course = await courseService.GetByIdAsync(vm.CourseId);
            ViewBag.Course = course.Value;
            return View(vm);
        }

        return RedirectToAction("Index", new { courseId = vm.CourseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid courseId)
    {
        await lessonService.DeleteAsync(id, UserId, IsAdmin);
        return RedirectToAction("Index", new { courseId });
    }
}
