using Application.Common;
using Application.DTOs.StudentDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RazorApp.Pages.Students;

// ── Index — список студентов (Admin) ──────────────────────────────────────────
[Authorize(Roles = UserRoles.Admin)]
public class IndexModel(IStudentService studentService) : PageModel
{
    public PagedResult<StudentDto> Students { get; set; } = new();
    [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;

    public async Task OnGetAsync()
    {
        var result = await studentService.GetAllAsync(CurrentPage, 10);
        Students = result.Value ?? new PagedResult<StudentDto>();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        await studentService.DeleteAsync(id);
        TempData["Success"] = "Student deleted.";
        return RedirectToPage();
    }
}

// ── Details ───────────────────────────────────────────────────────────────────
[Authorize]
public class DetailsModel(IStudentService studentService) : PageModel
{
    public StudentDto? Student { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var result = await studentService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        Student = result.Value;
        return Page();
    }
}

// ── Edit ──────────────────────────────────────────────────────────────────────
[Authorize]
public class EditModel(IStudentService studentService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string Id { get; set; } = string.Empty;
        [Required, MaxLength(200)] public string FullName { get; set; } = string.Empty;
        [MaxLength(1000)] public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole(UserRoles.Admin);
        if (!isAdmin && id != userId) return Forbid();

        var result = await studentService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var s = result.Value!;
        Input = new InputModel { Id = s.Id, FullName = s.FullName, Bio = s.Bio, AvatarUrl = s.AvatarUrl };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole(UserRoles.Admin);

        var result = await studentService.UpdateAsync(Input.Id, userId, isAdmin, new UpdateStudentDto
        {
            FullName = Input.FullName,
            Bio = Input.Bio,
            AvatarUrl = Input.AvatarUrl
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return Page();
        }

        return RedirectToPage("/Students/Details", new { id = Input.Id });
    }
}
