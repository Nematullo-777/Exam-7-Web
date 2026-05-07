using System;

namespace OnlineCourses.Application.DTOs.Students.Request;

public class UpdateStudentDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
