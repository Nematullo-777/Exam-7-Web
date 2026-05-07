// Models matching OnlineCourses.WebApi responses
namespace NewRazorApp.Models;

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? ThumbnailPath { get; set; }
    public decimal Price { get; set; }
    public string Level { get; set; } = "";
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public int LessonCount { get; set; }
    public double AverageRating { get; set; }
}

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}

public class LessonDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = "";
}

public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterDto
{
    public string FullName { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
}

public class LoginResponse
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Role { get; set; } = "";
    public string Token { get; set; } = "";
}

public class CreateCourseDto
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Level { get; set; }
    public Guid CategoryId { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = "";
    public int ProgressPercent { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = "";
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
}

public class UpdateStudentDto
{
    public string FullName { get; set; } = "";
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
}

public class StudentDto
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int EnrollmentCount { get; set; }
    public string Role { get; set; } = "";
}

public class CreateLessonDto
{
    public string Title { get; set; } = "";
    public string? Content { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public int DurationMinutes { get; set; }
}

public class UpdateLessonDto
{
    public string Title { get; set; } = "";
    public string? Content { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public int DurationMinutes { get; set; }
}
