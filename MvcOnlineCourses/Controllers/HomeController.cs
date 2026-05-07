using Microsoft.AspNetCore.Mvc;
using NewMvcApp.Services;

namespace NewMvcApp.Controllers;

public class HomeController : Controller
{
    private readonly ApiService _api;
    public HomeController(ApiService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var courses = await _api.GetCoursesAsync();
        return View(courses?.Data ?? new());
    }
}
