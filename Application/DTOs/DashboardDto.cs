namespace Application.DTOs;

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
    public string Title { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
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
    public string MonthName { get; set; } = string.Empty;
    public int NewEnrollments { get; set; }
    public int Completions { get; set; }
    public decimal Revenue { get; set; }
}

public class CategoryRevenueDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int CourseCount { get; set; }
    public int TotalStudents { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
}

public class CompletionRateDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalEnrolled { get; set; }
    public int TotalCompleted { get; set; }
    public double CompletionRatePercent { get; set; }
    public double AverageProgressPercent { get; set; }
}

public class InstructorStatsDto
{
    public string InstructorName { get; set; } = string.Empty;
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
    public string StudentId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
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
