using Application.DTOs.LessonDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RazorApp.Pages.Lessons;

[Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Instructor}")]
public class CreateModel(ILessonService lessonService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    [BindProperty(SupportsGet = true)] public Guid CourseId { get; set; }

    public class InputModel
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Content { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await lessonService.CreateAsync(CourseId, userId, new CreateLessonDto
        {
            Title = Input.Title, Content = Input.Content,
            VideoUrl = Input.VideoUrl, Order = Input.Order
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return Page();
        }

        return RedirectToPage("/Lessons/Index", new { CourseId });
    }
}

[Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Instructor}")]
public class EditModel(ILessonService lessonService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    [BindProperty(SupportsGet = true)] public Guid CourseId { get; set; }

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Content { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var result = await lessonService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var l = result.Value!;
        Input = new InputModel
        {
            Id = l.Id, Title = l.Title, Content = l.Content,
            VideoUrl = l.VideoUrl, Order = l.Order
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole(UserRoles.Admin);
        var result = await lessonService.UpdateAsync(Input.Id, userId, isAdmin, new UpdateLessonDto
        {
            Title = Input.Title, Content = Input.Content,
            VideoUrl = Input.VideoUrl, Order = Input.Order
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return Page();
        }

        return RedirectToPage("/Lessons/Index", new { CourseId });
    }
}
