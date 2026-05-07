using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly ApiService _api;
    public List<CategoryDto> Categories { get; set; } = new();
    public IndexModel(ApiService api) => _api = api;

    public async Task OnGetAsync() { var r = await _api.GetCategoriesAsync(); Categories = r?.Data ?? new(); }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        await _api.DeleteCategoryAsync(id);
        return RedirectToPage();
    }
}
