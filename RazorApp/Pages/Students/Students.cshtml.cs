using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Students;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApiService _api;
    public List<StudentDto> Students { get; set; } = new();

    public IndexModel(ApiService api) => _api = api;

    public async Task OnGetAsync()
    {
        var result = await _api.GetStudentsAsync();
        Students = result?.Data ?? new();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        await _api.DeleteStudentAsync(id);
        TempData["Success"] = "Студент удалён.";
        return RedirectToPage();
    }
}

public class DetailsModel : PageModel
{
    private readonly ApiService _api;
    public StudentDto? Student { get; set; }

    public DetailsModel(ApiService api) => _api = api;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var result = await _api.GetStudentAsync(id);
        if (result?.IsSuccess != true) return NotFound();
        Student = result.Data;
        return Page();
    }
}

public class EditModel : PageModel
{
    private readonly ApiService _api;
    [BindProperty] public InputModel Input { get; set; } = new();

    public EditModel(ApiService api) => _api = api;

    public class InputModel
    {
        public string Id { get; set; } = string.Empty;
        [Required, MaxLength(200)] public string FullName { get; set; } = string.Empty;
        [MaxLength(1000)] public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var result = await _api.GetStudentAsync(id);
        if (result?.IsSuccess != true) return NotFound();

        var s = result.Data!;
        Input = new InputModel { Id = s.Id, FullName = s.FullName, Bio = s.Bio, AvatarUrl = s.AvatarUrl };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _api.UpdateStudentAsync(Input.Id, new UpdateStudentDto
        {
            FullName = Input.FullName,
            Bio = Input.Bio,
            AvatarUrl = Input.AvatarUrl
        });

        if (!result?.IsSuccess == true)
        {
            ModelState.AddModelError("", result?.Error ?? "Ошибка");
            return Page();
        }

        return RedirectToPage("/Students/Details", new { id = Input.Id });
    }
}