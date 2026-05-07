using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly ApiService _api;
    public string? Error { get; set; }
    public LoginModel(ApiService api) => _api = api;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string email, string password)
    {
        var r = await _api.LoginAsync(new LoginDto { Email = email, Password = password });
        if (r?.IsSuccess == true && r.Data != null)
        {
            HttpContext.Session.SetString("jwt", r.Data.Token);
            HttpContext.Session.SetString("userName", r.Data.FullName);
            HttpContext.Session.SetString("role", r.Data.Role);
            return RedirectToPage("/Index");
        }
        Error = r?.Error ?? "Ошибка входа";
        return Page();
    }
}
