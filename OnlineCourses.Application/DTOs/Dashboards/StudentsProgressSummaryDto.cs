namespace OnlineCourses.Application.DTOs.Dashboards;

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