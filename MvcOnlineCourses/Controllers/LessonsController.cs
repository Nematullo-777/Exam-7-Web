using Application.DTOs.LessonDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcOnlineCourses.ViewModels;
using System.Security.Claims;

namespace MvcOnlineCourses.Controllers;

[Authorize]
public class LessonsController(
    ILessonService lessonService,
    ICourseService courseService,
    ILessonRepository lessonRepository,
    ICourseRepository courseRepository) : Controller
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

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLessonViewModel vm)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Index", new { courseId = vm.CourseId });

        await lessonService.CreateAsync(vm.CourseId, UserId, new CreateLessonDto
        {
            Title = vm.Title,
            Content = vm.Content,
            VideoUrl = vm.VideoUrl,
            Order = vm.Order,
            DurationMinutes = vm.DurationMinutes
        });

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
