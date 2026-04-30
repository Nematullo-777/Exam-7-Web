using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace RazorApp.Pages.Account;

[AllowAnonymous]
public class RegisterModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Student";
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await authService.RegisterAsync(new RegisterDto
        {
            FullName = Input.FullName,
            Email = Input.Email,
            Password = Input.Password,
            Role = Input.Role
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Registration failed.");
            return Page();
        }

        return RedirectToPage("/Account/Login");
    }
}
