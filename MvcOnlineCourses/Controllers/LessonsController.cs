using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcOnlineCourses.ViewModels;
using System.Security.Claims;
using NewMvcApp.Models;
using NewMvcApp.Services;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Constants;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class LessonsController(
    ILessonService lessonService,
    ICourseService courseService) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);
    private bool IsInstructor => User.IsInRole(UserRoles.Instructor);

    private readonly ApiService _api;

    public async Task<IActionResult> Index(Guid courseId)
    {
        var course = await _api.GetCourseAsync(courseId);
        if (course?.IsSuccess != true) return NotFound();

        var lessons = await _api.GetLessonsAsync(courseId);
        ViewBag.Course = course.Data;
        return View(lessons?.Data ?? new());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid courseId)
    {
        var course = await _api.GetCourseAsync(courseId);
        if (course?.IsSuccess != true) return NotFound();
        ViewBag.Course = course.Data;
        return View(new CreateLessonViewModel { CourseId = courseId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLessonViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var c = await _api.GetCourseAsync(vm.CourseId);
            ViewBag.Course = c?.Data;
            return View(vm);
        }

        var result = await _api.CreateLessonAsync(vm.CourseId, new CreateLessonDto
        {
            Title = vm.Title,
            Content = vm.Content,
            VideoUrl = vm.VideoUrl,
            Order = vm.Order,
            DurationMinutes = vm.DurationMinutes
        });

        if (result?.IsSuccess == true) return RedirectToAction("Index", new { courseId = vm.CourseId });

        ModelState.AddModelError("", result?.Error ?? "Ошибка");
        var course = await _api.GetCourseAsync(vm.CourseId);
        ViewBag.Course = course?.Data;
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, Guid courseId)
    {
        var result = await _api.GetLessonAsync(id);
        if (result?.IsSuccess != true) return NotFound();

        var lesson = result.Data!;
        return View(new EditLessonViewModel
        {
            Id = lesson.Id,
            CourseId = courseId,
            Title = lesson.Title,
            Content = lesson.Description ?? "",
            VideoUrl = lesson.VideoUrl,
            Order = lesson.Order,
            DurationMinutes = 0
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditLessonViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _api.UpdateLessonAsync(vm.Id, new UpdateLessonDto
        {
            Title = vm.Title,
            Content = vm.Content,
            VideoUrl = vm.VideoUrl,
            Order = vm.Order,
            DurationMinutes = vm.DurationMinutes
        });

        if (result?.IsSuccess == true) return RedirectToAction("Index", new { courseId = vm.CourseId });

        ModelState.AddModelError("", result?.Error ?? "Ошибка");
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id, Guid courseId)
    {
        await _api.DeleteLessonAsync(id);
        return RedirectToAction("Index", new { courseId });
    }
}
