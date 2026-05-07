
using OnlineCourses.Domain.Constants;

namespace MyBlazorApp.Models;

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}

public enum CourseLevel
{
    Beginner = 0,
    Intermediate = 1,
    Advances = 2,
    Expert
}

public enum EnrollmentStatus
{
    Active = 0,
    Completed = 1,
    Canceled = 2
}

public class LoginResponseDto
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Role { get; set; } = "";
    public string Token { get; set; } = "";
}

public class RegisterResponseDto
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Role { get; set; } = "";
}

// Запросы
public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
    public string Role { get; set; } = "Student";
}

public class SendEmailDto
{
    public string Email { get; set; } = "";
}

public class VerifyCodeDto
{
    public string Email { get; set; } = ""; public string Code { get; set; } = "";
}

public class ResetPasswordDto
{
    public string Email { get; set; } = ""; 
    public string NewPassword { get; set; } = ""; 
    public string ConfirmPassword { get; set; } = "";
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = ""; 
    public string NewPassword { get; set; } = "";
    public string ConfirmNewPassword { get; set; } = "";
}

public class GetCourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? ThumbnailPath { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; } = "";
    public Guid CategoryId { get; set; }
    public string InstructorName { get; set; } = "";
    public int LessonCount { get; set; }
    public double AverageRating { get; set; }
}

public class CreateCourseResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateCourseResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public Guid CategoryId { get; set; }
}

public class DeleteCourseResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
}

public class CreateCourseDto
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public Guid CategoryId { get; set; }
}

public class UpdateCourseDto
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public Guid CategoryId { get; set; }
}

public class GetCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}

public class CreateCategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = ""; 
    public string? Description { get; set; }
}

public class UpdateCategoryDto
{
    public string Name { get; set; } = ""; 
    public string? Description { get; set; }
}

public class GetLessonDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public int DurationMinutes { get; set; }
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public string? MaterialPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = "";
}

public class CreateLessonResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public int Order { get; set; }
    public Guid CourseId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateLessonResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public string? MaterialPath { get; set; }
    public Guid CourseId { get; set; }
}

public class DeleteLessonResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
}

public class CreateLessonDto
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public string? MaterialPath { get; set; }
    public Guid CourseId { get; set; }
}

public class UpdateLessonDto
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public string? MaterialPath { get; set; }
}

public class GetEnrollmentDto
{
    public Guid Id { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public EnrollmentStatus Status { get; set; }
    public int ProgressPercent { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = "";
    public decimal CoursePrice { get; set; }
    public string StudentId { get; set; } = "";
    public string StudentName { get; set; } = "";
}

public class CreateEnrollmentResponseDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = "";
    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; }
}

public class DeleteEnrollmentResponseDto
{
    public Guid Id { get; set; }
    public string CourseTitle { get; set; } = "";
}

public class CreateEnrollmentDto
{
    public Guid CourseId { get; set; }
}

public class UpdateEnrollmentDto
{
    public int ProgressPercent { get; set; }
}

public class GetReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = "";
    public string StudentId { get; set; } = "";
    public string StudentName { get; set; } = "";
}

public class CreateReviewResponseDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CourseId { get; set; }
    public string StudentId { get; set; } = "";
}

public class UpdateReviewResponseDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public Guid CourseId { get; set; }
}

public class DeleteReviewResponseDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class CreateReviewDto
{
    public int Rating { get; set; } 
    public string? Comment { get; set; }
}

public class UpdateReviewDto
{
    public int Rating { get; set; } 
    public string? Comment { get; set; }
}

public class GetStudentDto
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Bio { get; set; } = "";
    public int EnrollmentCount { get; set; }
    public string Role { get; set; } = "";
    public string? ImageUrl { get; set; }
}

public class UpdateStudentResponseDto
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Bio { get; set; } = "";
    public int EnrollmentCount { get; set; }
    public string Role { get; set; } = "";
    public string? ImageUrl { get; set; }
}

public class UpdateStudentDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
}

public class DashboardSummaryDto
{
    public int TotalCourses { get; set; }
    public int PublishedCourses { get; set; }
    public int TotalStudents { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AveragePlatformRating { get; set; }
    public int TotalReviews { get; set; }
}

public class TopCourseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public int EnrollmentCount { get; set; }
    public int CompletedCount { get; set; }
    public double CompletionRate { get; set; }
    public double AverageRating { get; set; }
    public decimal Revenue { get; set; }
}

public class MonthlyEnrollmentDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = "";
    public int NewEnrollments { get; set; }
    public int Completions { get; set; }
    public decimal Revenue { get; set; }
}

public class CategoryRevenueDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public int CourseCount { get; set; }
    public int TotalStudents { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
}

public class CompletionRateDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = "";
    public int TotalEnrolled { get; set; }
    public int TotalCompleted { get; set; }
    public double CompletionRatePercent { get; set; }
    public double AverageProgressPercent { get; set; }
}

public class InstructorStatsDto
{
    public string InstructorName { get; set; } = "";
    public int CourseCount { get; set; }
    public int PublishedCourseCount { get; set; }
    public int TotalStudents { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<TopCourseDto> TopCourses { get; set; } = new();
    public List<MonthlyEnrollmentDto> EnrollmentTrend { get; set; } = new();
}

public class StudentsProgressSummaryDto
{
    public int TotalStudents { get; set; }
    public int StudentsWithActiveEnrollment { get; set; }
    public int StudentsCompletedAtLeastOne { get; set; }
    public int StudentsNeverStarted { get; set; }
    public double AverageCoursesPerStudent { get; set; }
    public List<StudentProgressDto> TopActiveStudents { get; set; } = new();
}

public class StudentProgressDto
{
    public string StudentId { get; set; } = "";
    public string FullName { get; set; } = "";
    public int CompletedCourses { get; set; }
    public int ActiveEnrollments { get; set; }
    public double AverageProgress { get; set; }
}

public class RatingsDistributionDto
{
    public int OneStar { get; set; }
    public int TwoStars { get; set; }
    public int ThreeStars { get; set; }
    public int FourStars { get; set; }
    public int FiveStars { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}
public class LoginResponse
{
    public string Token { get; set; } = "";

    public string Id { get; set; } = "";

    public string FullName { get; set; } = "";

    public string Email { get; set; } = "";

    public string UserName { get; set; } = "";

    public string Role { get; set; } = UserRoles.Student;
}

public class ResultDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}