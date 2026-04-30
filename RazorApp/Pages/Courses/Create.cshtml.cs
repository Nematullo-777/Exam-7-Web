using Application.DTOs.CategoryDTOs;
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

[Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Admin}")]
public class CreateModel(ICourseService courseService, ICategoryService categoryService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();
    public SelectList? CategoryOptions { get; set; }

    public class InputModel
    {
        [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Range(0, double.MaxValue)] public decimal Price { get; set; }
        public CourseLevel Level { get; set; }
        [Required] public Guid CategoryId { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }

    public async Task OnGetAsync()
    {
        await PopulateCategoriesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) { await PopulateCategoriesAsync(); return Page(); }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await courseService.CreateAsync(userId, new CreateCourseDto
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

        if (Input.Thumbnail is not null)
            await courseService.UploadThumbnailAsync(result.Value!.Id, userId, Input.Thumbnail);

        return RedirectToPage("/Courses/Index");
    }

    private async Task PopulateCategoriesAsync()
    {
        var cats = await categoryService.GetAllAsync();
        CategoryOptions = new SelectList(cats.Value, "Id", "Name");
    }
}
