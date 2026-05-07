using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using NewRazorApp.Models;
using NewRazorApp.Services;
using OnlineCourses.Domain.Constants;

namespace NewRazorApp.Pages.Lessons;

[Authorize(Roles = $"{UserRoles.Instructor},{UserRoles.Admin}")]
public class CreateModel : PageModel
{
    private readonly ApiService _api;
    [BindProperty] public InputModel Input { get; set; } = new();
    [BindProperty(SupportsGet = true)] public Guid CourseId { get; set; }

    public CreateModel(ApiService api) => _api = api;

    public class InputModel
    {
        [Required] public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public int DurationMinutes { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _api.CreateLessonAsync(CourseId, new CreateLessonDto
        {
            Title = Input.Title,
            Content = Input.Content,
            VideoUrl = Input.VideoUrl,
            Order = Input.Order,
            DurationMinutes = Input.DurationMinutes
        });

        if (!result?.IsSuccess == true)
        {
            ModelState.AddModelError("", result?.Error ?? "Ошибка");
            return Page();
        }

        return RedirectToPage("/Lessons/Index", new { CourseId });
    }
}

public class EditModel : PageModel
{
    private readonly ApiService _api;
    [BindProperty] public InputModel Input { get; set; } = new();
    [BindProperty(SupportsGet = true)] public Guid CourseId { get; set; }

    public EditModel(ApiService api) => _api = api;

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public int DurationMinutes { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var result = await _api.GetLessonAsync(id);
        if (result?.IsSuccess != true) return NotFound();

        var l = result.Data!;
        Input = new InputModel
        {
            Id = l.Id,
            Title = l.Title,
            Content = l.Description,
            VideoUrl = l.VideoUrl,
            Order = l.Order
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _api.UpdateLessonAsync(Input.Id, new UpdateLessonDto
        {
            Title = Input.Title,
            Content = Input.Content,
            VideoUrl = Input.VideoUrl,
            Order = Input.Order,
            DurationMinutes = Input.DurationMinutes
        });

        if (!result?.IsSuccess == true)
        {
            ModelState.AddModelError("", result?.Error ?? "Ошибка");
            return Page();
        }

        return RedirectToPage("/Lessons/Index", new { CourseId });
    }
}