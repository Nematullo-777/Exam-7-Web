using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public void OnGet() { }
}
