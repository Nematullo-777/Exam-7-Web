using Application.Common;
using Application.DTOs.CategoryDTOs;
using Application.DTOs.CourseDTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorApp.Pages.Courses;

public class IndexModel(ICourseService courseService, ICategoryService categoryService) : PageModel
{
    public PagedResult<CourseDto> Courses { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();

    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    [BindProperty(SupportsGet = true)] public Guid? CategoryId { get; set; }
    [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;

    public async Task OnGetAsync()
    {
        var filter = new CourseFilterDto
        {
            Search = Search,
            CategoryId = CategoryId,
            Page = CurrentPage,
            PageSize = 10,
            IsPublished = true
        };

        var result = await courseService.GetAllAsync(filter);
        Courses = result.Value ?? new PagedResult<CourseDto>();

        var cats = await categoryService.GetAllAsync();
        Categories = cats.Value ?? new List<CategoryDto>();
    }
}
