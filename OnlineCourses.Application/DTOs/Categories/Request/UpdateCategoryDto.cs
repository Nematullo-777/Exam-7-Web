using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCourses.Application.DTOs.Categories.Request;

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
