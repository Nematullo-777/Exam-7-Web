using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcOnlineCourses.ViewModels;

namespace MvcOnlineCourses.Controllers;

public class AccountController(IAuthService authService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await authService.LoginAsync(new LoginDto
        {
            Email = model.Email,
            Password = model.Password
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", "Неверный email или пароль");
            return View(model);
        }

        Response.Cookies.Append("jwt", result.Value!, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // в dev false, в prod true
            Expires = DateTime.UtcNow.AddHours(3)
        });

        return RedirectToAction("Index", "Courses");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await authService.RegisterAsync(new RegisterDto
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            Role = model.Role
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error ?? "Ошибка регистрации");
            return View(model);
        }

        // 🔥 АВТО-ЛОГИН (вставить сюда)
        var loginResult = await authService.LoginAsync(new LoginDto
        {
            Email = model.Email,
            Password = model.Password
        });

        if (loginResult.IsSuccess)
        {
            Response.Cookies.Append("jwt", loginResult.Value!, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                Expires = DateTime.UtcNow.AddHours(3)
            });

            // 🔥 редирект на главную
            return RedirectToAction("Index", "Courses");
        }

        // если вдруг логин не прошёл
        return RedirectToAction("Login");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return RedirectToAction("Login");
    }
}
