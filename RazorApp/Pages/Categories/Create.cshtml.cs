using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Categories;

public class CreateModel : PageModel
{
    private readonly ApiService _api;
    public string? Error { get; set; }
    public CreateModel(ApiService api) => _api = api;
    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string name, string? description)
    {
        var r = await _api.CreateCategoryAsync(new CreateCategoryDto { Name = name, Description = description });
        if (r?.IsSuccess == true) return RedirectToPage("/Categories/Index");
        Error = r?.Error ?? "Ошибка";
        return Page();
    }
}
