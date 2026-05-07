using Microsoft.AspNetCore.Mvc;
using NewMvcApp.Models;
using NewMvcApp.Services;

namespace NewMvcApp.Controllers;

public class CategoriesController : Controller
{
    private readonly ApiService _api;
    public CategoriesController(ApiService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var result = await _api.GetCategoriesAsync();
        return View(result?.Data ?? new());
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        var result = await _api.CreateCategoryAsync(dto);
        if (result?.IsSuccess == true) return RedirectToAction("Index");
        ViewBag.Error = result?.Error ?? "Ошибка";
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _api.DeleteCategoryAsync(id);
        return RedirectToAction("Index");
    }
}
