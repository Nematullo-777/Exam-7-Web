using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages;

public class IndexModel : PageModel
{
    private readonly ApiService _api;
    public List<CourseDto> Courses { get; set; } = new();

    public IndexModel(ApiService api) => _api = api;

    public async Task OnGetAsync()
    {
        var r = await _api.GetCoursesAsync();
        Courses = r?.Data ?? new();
    }
}
