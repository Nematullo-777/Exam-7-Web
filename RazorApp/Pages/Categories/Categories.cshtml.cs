using Application.DTOs.CategoryDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace RazorApp.Pages.Categories;

[Authorize(Roles = UserRoles.Admin)]
public class IndexModel(ICategoryService categoryService) : PageModel
{
    public List<CategoryDto> Categories { get; set; } = new();

    public async Task OnGetAsync()
    {
        var result = await categoryService.GetAllAsync();
        Categories = result.Value ?? new List<CategoryDto>();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var result = await categoryService.DeleteAsync(id);
        if (!result.IsSuccess)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = "Category deleted.";

        return RedirectToPage();
    }
}

[Authorize(Roles = UserRoles.Admin)]
public class CreateModel(ICategoryService categoryService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        [MaxLength(500)] public string? Description { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await categoryService.CreateAsync(new CreateCategoryDto
        {
            Name = Input.Name,
            Description = Input.Description
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return Page();
        }

        return RedirectToPage("/Categories/Index");
    }
}

[Authorize(Roles = UserRoles.Admin)]
public class EditModel(ICategoryService categoryService) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
        [MaxLength(500)] public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var result = await categoryService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var c = result.Value!;
        Input = new InputModel { Id = c.Id, Name = c.Name, Description = c.Description };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await categoryService.UpdateAsync(Input.Id, new UpdateCategoryDto
        {
            Name = Input.Name,
            Description = Input.Description
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error!);
            return Page();
        }

        return RedirectToPage("/Categories/Index");
    }
}
