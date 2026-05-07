using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NewRazorApp.Pages.Auth;

public class LogoutModel : PageModel
{
    public IActionResult OnGet() { HttpContext.Session.Clear(); return RedirectToPage("/Index"); }
}
