using Microsoft.AspNetCore.Mvc;
using NewMvcApp.Models;
using NewMvcApp.Services;

namespace NewMvcApp.Controllers;

public class CoursesController : Controller
{
    private readonly ApiService _api;
    public CoursesController(ApiService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var result = await _api.GetCoursesAsync();
        return View(result?.Data ?? new());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var course = await _api.GetCourseAsync(id);
        if (course?.IsSuccess != true) return NotFound();
        var lessons = await _api.GetLessonsAsync(id);
        ViewBag.Lessons = lessons?.Data ?? new();
        return View(course.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var cats = await _api.GetCategoriesAsync();
        ViewBag.Categories = cats?.Data ?? new();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        var result = await _api.CreateCourseAsync(dto);
        if (result?.IsSuccess == true)
            return RedirectToAction("Index");
        ViewBag.Error = result?.Error ?? "Ошибка";
        var cats = await _api.GetCategoriesAsync();
        ViewBag.Categories = cats?.Data ?? new();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteCourseAsync(id);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> TogglePublish(Guid id)
    {
        await _api.TogglePublishAsync(id);
        return RedirectToAction("Index");
    }
}
