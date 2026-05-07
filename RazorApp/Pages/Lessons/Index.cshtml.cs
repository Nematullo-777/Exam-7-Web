using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Lessons;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApiService _api;
    public List<LessonDto> Lessons { get; set; } = new();
    [BindProperty(SupportsGet = true)] public Guid CourseId { get; set; }

    public IndexModel(ApiService api) => _api = api;

    public async Task OnGetAsync()
    {
        var result = await _api.GetLessonsAsync(CourseId);
        Lessons = result?.Data ?? new();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await _api.DeleteLessonAsync(id);
        return RedirectToPage(new { CourseId });
    }
}
