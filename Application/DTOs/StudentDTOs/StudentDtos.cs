using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.StudentDTOs;

public class StudentDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int EnrollmentCount { get; set; }
}

public class UpdateStudentDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }
}
