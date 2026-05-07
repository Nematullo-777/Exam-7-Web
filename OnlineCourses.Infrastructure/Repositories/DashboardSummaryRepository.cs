using System;
using Microsoft.EntityFrameworkCore;
using OnlineCourses.Application.DTOs.Dashboards;
using OnlineCourses.Application.Interfaces.Repositories;
using OnlineCourses.Domain.Enums;
using OnlineCourses.Domain.Identity;
using OnlineCourses.Infrastructure.Data;

namespace OnlineCourses.Infrastructure.Repositories;

public class DashboardSummaryRepository : IDashboardSummaryRepository
{
    private readonly AppDbContext _context;

    public DashboardSummaryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTotalCoursesAsync()
    {
        return await _context.Courses.CountAsync();
    }

    public async Task<int> GetPublishedCoursesAsync() =>
        await _context.Courses.CountAsync(c => c.IsPublished);

    public async Task<int> GetTotalStudentsAsync() =>
        await _context.UserRoles
            .Join(_context.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => new { ur.UserId, r.Name })
            .CountAsync(x => x.Name == "Student");

    public async Task<int> GetTotalInstructorsAsync() =>
        await _context.UserRoles
            .Join(_context.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => new { ur.UserId, r.Name })
            .CountAsync(x => x.Name == "Instructor");

    public async Task<int> GetTotalEnrollmentsAsync() =>
        await _context.Enrollments.CountAsync();

    public async Task<int> GetActiveEnrollmentsAsync() =>
        await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active);

    public async Task<int> GetCompletedEnrollmentsAsync() =>
        await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Completed);

    public async Task<decimal> GetTotalRevenueAsync() =>
        await _context.Enrollments.SumAsync(e => e.Course.Price);

    public async Task<int> GetTotalReviewsAsync() =>
        await _context.Reviews.CountAsync();

    public async Task<double> GetAverageRatingAsync() =>
        await _context.Reviews.AnyAsync()
            ? await _context.Reviews.AverageAsync(r => (double)r.Rating)
            : 0;

    public async Task<List<TopCourseDto>> GetTopCoursesAsync(int count) =>
        await _context.Enrollments
            .GroupBy(e => new
            {
                e.CourseId,
                e.Course.Title,
                e.Course.Price,
                InstructorName = e.Course.Instructor.FullName
            })
            .Select(g => new TopCourseDto
            {
                CourseId = g.Key.CourseId,
                Title = g.Key.Title,
                InstructorName = g.Key.InstructorName,
                EnrollmentCount = g.Count(),
                CompletedCount = g.Count(e => e.Status == EnrollmentStatus.Completed),
                CompletionRate = g.Count() == 0 ? 0 :
                    (double)g.Count(e => e.Status == EnrollmentStatus.Completed) / g.Count() * 100,
                AverageRating = _context.Reviews
                    .Where(r => r.CourseId == g.Key.CourseId)
                    .Average(r => (double?)r.Rating) ?? 0,
                Revenue = g.Count() * g.Key.Price
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(count)
            .ToListAsync();

    public async Task<List<MonthlyEnrollmentDto>> GetEnrollmentsByMonthAsync()
    {
        var data = await _context.Enrollments
            .Where(e => e.EnrolledAt >= DateTime.UtcNow.AddMonths(-12))
            .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
            .Select(g => new MonthlyEnrollmentDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                NewEnrollments = g.Count(),
                Completions = g.Count(e => e.Status == EnrollmentStatus.Completed),
                Revenue = g.Sum(e => e.Course.Price)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync();

        var monthNames = new[]
        {
        "Январь", "Февраль", "Март", "Апрель",
        "Май", "Июнь", "Июль", "Август",
        "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
    };

        foreach (var item in data)
            item.MonthName = monthNames[item.Month - 1];

        return data;
    }
    public async Task<List<CategoryRevenueDto>> GetRevenueByCategoryAsync() =>
    await _context.Categories
        .Select(cat => new CategoryRevenueDto
        {
            CategoryId = cat.Id,
            CategoryName = cat.Name,
            CourseCount = cat.Courses.Count,
            TotalStudents = cat.Courses.SelectMany(c => c.Enrollments).Count(),
            TotalRevenue = cat.Courses.SelectMany(c => c.Enrollments).Sum(e => (decimal?)e.Course.Price) ?? 0,
            AverageRating = cat.Courses
                .SelectMany(c => c.Reviews)
                .Average(r => (double?)r.Rating) ?? 0
        })
        .OrderByDescending(x => x.TotalRevenue)
        .ToListAsync();

public async Task<List<CompletionRateDto>> GetCompletionRateAsync() =>
    await _context.Courses
        .Where(c => c.Enrollments.Count >= 5)
        .Select(c => new CompletionRateDto
        {
            CourseId = c.Id,
            Title = c.Title,
            TotalEnrolled = c.Enrollments.Count,
            TotalCompleted = c.Enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
            CompletionRatePercent = c.Enrollments.Count == 0 ? 0 :
                (double)c.Enrollments.Count(e => e.Status == EnrollmentStatus.Completed) / c.Enrollments.Count * 100,
            AverageProgressPercent = c.Enrollments.Average(e => (double?)e.ProgressPercent) ?? 0
        })
        .ToListAsync();

public async Task<InstructorStatsDto> GetInstructorStatsAsync(string instructorId)
{
    var instructor = await _context.Users.FindAsync(instructorId);
    var courses = await _context.Courses
        .Where(c => c.InstructorId == instructorId)
        .Include(c => c.Enrollments)
        .Include(c => c.Reviews)
        .ToListAsync();

    var topCourses = courses
        .OrderByDescending(c => c.Enrollments.Count)
        .Take(3)
        .Select(c => new TopCourseDto
        {
            CourseId = c.Id,
            Title = c.Title,
            InstructorName = instructor?.FullName ?? string.Empty,
            EnrollmentCount = c.Enrollments.Count,
            CompletedCount = c.Enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
            CompletionRate = c.Enrollments.Count == 0 ? 0 :
                (double)c.Enrollments.Count(e => e.Status == EnrollmentStatus.Completed) / c.Enrollments.Count * 100,
            AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => (double)r.Rating) : 0,
            Revenue = c.Enrollments.Count * c.Price
        }).ToList();

    var trend = await _context.Enrollments
        .Where(e => e.Course.InstructorId == instructorId && e.EnrolledAt >= DateTime.UtcNow.AddMonths(-6))
        .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
        .Select(g => new MonthlyEnrollmentDto
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            NewEnrollments = g.Count(),
            Completions = g.Count(e => e.Status == EnrollmentStatus.Completed),
            Revenue = g.Sum(e => e.Course.Price)
        })
        .OrderBy(x => x.Year).ThenBy(x => x.Month)
        .ToListAsync();

    var monthNames = new[] { "Январь","Февраль","Март","Апрель","Май","Июнь","Июль","Август","Сентябрь","Октябрь","Ноябрь","Декабрь" };
    foreach (var item in trend) item.MonthName = monthNames[item.Month - 1];

    return new InstructorStatsDto
    {
        InstructorName = instructor?.FullName ?? string.Empty,
        CourseCount = courses.Count,
        PublishedCourseCount = courses.Count(c => c.IsPublished),
        TotalStudents = courses.Sum(c => c.Enrollments.Count),
        TotalReviews = courses.Sum(c => c.Reviews.Count),
        AverageRating = courses.SelectMany(c => c.Reviews).Any()
            ? courses.SelectMany(c => c.Reviews).Average(r => (double)r.Rating) : 0,
        TotalRevenue = courses.Sum(c => c.Enrollments.Count * c.Price),
        TopCourses = topCourses,
        EnrollmentTrend = trend
    };
}

public async Task<StudentsProgressSummaryDto> GetStudentsProgressAsync()
{
    var studentRoleId = await _context.Roles
        .Where(r => r.Name == "Student")
        .Select(r => r.Id)
        .FirstOrDefaultAsync();

    var studentIds = await _context.UserRoles
        .Where(ur => ur.RoleId == studentRoleId)
        .Select(ur => ur.UserId)
        .ToListAsync();

    var total = studentIds.Count;

    var withActive = await _context.Enrollments
        .Where(e => studentIds.Contains(e.StudentId) && e.Status == EnrollmentStatus.Active)
        .Select(e => e.StudentId).Distinct().CountAsync();

    var withCompleted = await _context.Enrollments
        .Where(e => studentIds.Contains(e.StudentId) && e.Status == EnrollmentStatus.Completed)
        .Select(e => e.StudentId).Distinct().CountAsync();

    var enrolledIds = await _context.Enrollments
        .Where(e => studentIds.Contains(e.StudentId))
        .Select(e => e.StudentId).Distinct().ToListAsync();

    var neverStarted = studentIds.Count(id => !enrolledIds.Contains(id));

    var totalEnrollments = await _context.Enrollments
        .Where(e => studentIds.Contains(e.StudentId)).CountAsync();

    var avgCourses = total == 0 ? 0 : (double)totalEnrollments / total;

    var top5 = await _context.Enrollments
        .Where(e => studentIds.Contains(e.StudentId))
        .GroupBy(e => new { e.StudentId, e.Student.FullName })
        .Select(g => new StudentProgressDto
        {
            StudentId = g.Key.StudentId,
            FullName = g.Key.FullName,
            CompletedCourses = g.Count(e => e.Status == EnrollmentStatus.Completed),
            ActiveEnrollments = g.Count(e => e.Status == EnrollmentStatus.Active),
            AverageProgress = g.Average(e => (double)e.ProgressPercent)
        })
        .OrderByDescending(x => x.CompletedCourses)
        .Take(5)
        .ToListAsync();

    return new StudentsProgressSummaryDto
    {
        TotalStudents = total,
        StudentsWithActiveEnrollment = withActive,
        StudentsCompletedAtLeastOne = withCompleted,
        StudentsNeverStarted = neverStarted,
        AverageCoursesPerStudent = avgCourses,
        TopActiveStudents = top5
    };
}

public async Task<RatingsDistributionDto> GetRatingsDistributionAsync()
{
    var distribution = await _context.Reviews
        .GroupBy(r => r.Rating)
        .Select(g => new { Rating = g.Key, Count = g.Count() })
        .ToListAsync();

    var total = distribution.Sum(x => x.Count);
    var avg = total == 0 ? 0 : distribution.Sum(x => x.Rating * x.Count) / (double)total;

    return new RatingsDistributionDto
    {
        OneStar    = distribution.FirstOrDefault(x => x.Rating == 1)?.Count ?? 0,
        TwoStars   = distribution.FirstOrDefault(x => x.Rating == 2)?.Count ?? 0,
        ThreeStars = distribution.FirstOrDefault(x => x.Rating == 3)?.Count ?? 0,
        FourStars  = distribution.FirstOrDefault(x => x.Rating == 4)?.Count ?? 0,
        FiveStars  = distribution.FirstOrDefault(x => x.Rating == 5)?.Count ?? 0,
        AverageRating = avg,
        TotalReviews = total
    };
}
}
