using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly ApiService _api;
    public string? Error { get; set; }
    public bool Success { get; set; }
    public RegisterModel(ApiService api) => _api = api;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string fullName, string userName, string email, string password, string confirmPassword)
    {
        if (password != confirmPassword) { Error = "Пароли не совпадают"; return Page(); }
        var r = await _api.RegisterAsync(new RegisterDto { FullName = fullName, UserName = userName, Email = email, Password = password, ConfirmPassword = confirmPassword });
        if (r?.IsSuccess == true) { Success = true; return Page(); }
        Error = r?.Error ?? "Ошибка";
        return Page();
    }
}
