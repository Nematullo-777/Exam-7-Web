using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace RazorApp.Pages.Account;

[AllowAnonymous]
public class LoginModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await authService.LoginAsync(new LoginDto
        {
            Email = Input.Email,
            Password = Input.Password
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Invalid credentials.");
            return Page();
        }

        Response.Cookies.Append("jwt", result.Value!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return RedirectToPage("/Courses/Index");
    }
}
