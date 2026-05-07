using Microsoft.AspNetCore.Mvc;
using NewMvcApp.Models;
using NewMvcApp.Services;

namespace NewMvcApp.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _api;
    public AuthController(ApiService api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _api.LoginAsync(dto);
        if (result?.IsSuccess == true && result.Data != null)
        {
            HttpContext.Session.SetString("jwt", result.Data.Token);
            HttpContext.Session.SetString("userName", result.Data.FullName);
            HttpContext.Session.SetString("role", result.Data.Role);
            return RedirectToAction("Index", "Home");
        }
        ViewBag.Error = result?.Error ?? "Ошибка входа";
        return View(dto);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            ViewBag.Error = "Пароли не совпадают";
            return View(dto);
        }
        var result = await _api.RegisterAsync(dto);
        if (result?.IsSuccess == true)
            return RedirectToAction("Login");

        ViewBag.Error = result?.Error ?? "Ошибка регистрации";
        return View(dto);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
