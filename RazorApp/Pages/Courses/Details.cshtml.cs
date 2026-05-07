using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Courses;

public class DetailsModel : PageModel
{
    private readonly ApiService _api;
    public CourseDto? Course { get; set; }
    public List<LessonDto> Lessons { get; set; } = new();
    public DetailsModel(ApiService api) => _api = api;

    public async Task OnGetAsync(Guid id)
    {
        var r = await _api.GetCourseAsync(id);
        Course = r?.Data;
        var lr = await _api.GetLessonsAsync(id);
        Lessons = lr?.Data ?? new();
    }
}
