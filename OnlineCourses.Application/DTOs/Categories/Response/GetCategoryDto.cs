using System;

namespace OnlineCourses.Application.DTOs.Categories.Response;

public class GetCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
