using System;

namespace OnlineCourses.Application.DTOs.Students.Response;

public class GetStudentDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Bio { get; set; } =  string.Empty;
    public int EnrollmentCount { get; set; } = 0;
    public string Role { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
