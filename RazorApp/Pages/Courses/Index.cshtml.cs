using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Courses;

public class IndexModel : PageModel
{
    private readonly ApiService _api;
    public List<CourseDto> Courses { get; set; } = new();
    public IndexModel(ApiService api) => _api = api;

    public async Task OnGetAsync() { var r = await _api.GetCoursesAsync(); Courses = r?.Data ?? new(); }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _api.DeleteCourseAsync(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAsync(Guid id)
    {
        await _api.TogglePublishAsync(id);
        return RedirectToPage();
    }
}
