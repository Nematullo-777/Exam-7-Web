using Application.DTOs.LessonDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RazorApp.Pages.Lessons;

[Authorize]
public class IndexModel(ILessonService lessonService) : PageModel
{
    public List<LessonDto> Lessons { get; set; } = new();
    [BindProperty(SupportsGet = true)] public Guid CourseId { get; set; }

    public async Task OnGetAsync()
    {
        var result = await lessonService.GetByCourseIdAsync(CourseId);
        Lessons = result.Value ?? new List<LessonDto>();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole(UserRoles.Admin);
        await lessonService.DeleteAsync(id, userId, isAdmin);
        return RedirectToPage(new { CourseId });
    }
}
