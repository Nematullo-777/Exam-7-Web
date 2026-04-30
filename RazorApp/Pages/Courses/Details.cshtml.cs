using Application.DTOs.CourseDTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages.Courses;

public class DetailsModel(ICourseService courseService) : PageModel
{
    public CourseDto? Course { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var result = await courseService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        Course = result.Value;
        return Page();
    }
}
