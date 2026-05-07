using System;
using System.ComponentModel.DataAnnotations;
using OnlineCourses.Domain.Constants;

namespace OnlineCourses.Application.DTOs.Auth.Request;

public class RegisterDto
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(150)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(150)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Role = UserRoles.Student;
}
