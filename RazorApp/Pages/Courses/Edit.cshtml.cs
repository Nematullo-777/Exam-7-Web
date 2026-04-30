using Application.DTOs.CourseDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RazorApp.Pages.Courses;

[Authorize(Roles = UserRoles.Instructor)]
public class EditModel(ICourseService courseService, ICategoryService categoryService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public SelectList? CategoryOptions { get; set; }

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Range(0, double.MaxValue)] public decimal Price { get; set; }
        public CourseLevel Level { get; set; }
        [Required] public Guid CategoryId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var result = await courseService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var c = result.Value!;
        Input = new InputModel
        {
            Id = c.Id, Title = c.Title, Description = c.Description,
            Price = c.Price, Level = c.Level, CategoryId = c.CategoryId
        };

        await PopulateCategoriesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) { await PopulateCategoriesAsync(); return Page(); }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await courseService.UpdateAsync(Input.Id, userId, new UpdateCourseDto
        {
            Title = Input.Title, Description = Input.Description,
            Price = Input.Price, Level = Input.Level, CategoryId = Input.CategoryId
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            await PopulateCategoriesAsync();
            return Page();
        }

        return RedirectToPage("/Courses/Index");
    }

    private async Task PopulateCategoriesAsync()
    {
        var cats = await categoryService.GetAllAsync();
        CategoryOptions = new SelectList(cats.Value, "Id", "Name");
    }
}
