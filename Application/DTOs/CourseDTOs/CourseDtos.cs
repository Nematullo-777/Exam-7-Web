using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CourseDTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public string InstructorId { get; set; } = string.Empty;
    public int LessonCount { get; set; }
    public int EnrollmentCount { get; set; }
    public double AverageRating { get; set; }
}

public class CreateCourseDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public CourseLevel Level { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}

public class UpdateCourseDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public CourseLevel Level { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}

public class CourseFilterDto
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public CourseLevel? Level { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsPublished { get; set; }
    public string? SortBy { get; set; } // "price", "rating", "createdAt"
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
