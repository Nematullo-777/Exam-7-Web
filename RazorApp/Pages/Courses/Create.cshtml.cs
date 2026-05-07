using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Courses;

public class CreateModel : PageModel
{
    private readonly ApiService _api;
    public List<CategoryDto> Categories { get; set; } = new();
    public string? Error { get; set; }
    public CreateModel(ApiService api) => _api = api;

    public async Task OnGetAsync() { var r = await _api.GetCategoriesAsync(); Categories = r?.Data ?? new(); }

    public async Task<IActionResult> OnPostAsync(string title, string? description, decimal price, int level, Guid categoryId)
    {
        var r = await _api.CreateCourseAsync(new CreateCourseDto { Title = title, Description = description, Price = price, Level = level, CategoryId = categoryId });
        if (r?.IsSuccess == true) return RedirectToPage("/Courses/Index");
        Error = r?.Error ?? "Ошибка";
        var cats = await _api.GetCategoriesAsync();
        Categories = cats?.Data ?? new();
        return Page();
    }
}
