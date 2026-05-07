using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcOnlineCourses.ViewModels;
using NewMvcApp.Models;
using NewMvcApp.Services;
using OnlineCourses.Application.Interfaces.Services;

namespace MvcOnlineCourses.Controllers;

public class AccountController(IAuthService authService) : Controller
{
    private readonly ApiService _api;
    
    

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.LoginAsync(new NewMvcApp.Models.LoginDto
        {
            Email = model.Email,
            Password = model.Password
        });

        if (result?.IsSuccess == true && result.Data != null)
        {
            HttpContext.Session.SetString("jwt", result.Data.Token);
            HttpContext.Session.SetString("userName", result.Data.FullName);
            HttpContext.Session.SetString("role", result.Data.Role);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", result?.Error ?? "Неверный email или пароль");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.RegisterAsync(new NewMvcApp.Models.RegisterDto
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.Password
        });

        if (result?.IsSuccess == true)
        {
            var loginResult = await _api.LoginAsync(new NewMvcApp.Models.LoginDto
            {
                Email = model.Email,
                Password = model.Password
            });

            if (loginResult?.IsSuccess == true && loginResult.Data != null)
            {
                HttpContext.Session.SetString("jwt", loginResult.Data.Token);
                HttpContext.Session.SetString("userName", loginResult.Data.FullName);
                HttpContext.Session.SetString("role", loginResult.Data.Role);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", result?.Error ?? "Ошибка регистрации");
        return View(model);
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
