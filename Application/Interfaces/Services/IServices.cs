using Application.Common;
using Application.DTOs;
using Application.DTOs.AuthDTOs;
using Application.DTOs.CourseDTOs;
using Application.DTOs.EnrollmentDTOs;
using Application.DTOs.LessonDTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(LoginDto model);
    Task<Result<bool>> RegisterAsync(RegisterDto model);
    Task<Result<bool>> ChangePasswordAsync(string userId, ChangePasswordDto model);
    Task<Result<UserDto>> GetCurrentUserAsync(string userId);
    Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto model);
    Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto model);
}

public interface ICourseService
{
    Task<Result<PagedResult<CourseDto>>> GetAllAsync(CourseFilterDto filter);
    Task<Result<CourseDto>> GetByIdAsync(Guid id);
    Task<Result<CourseDto>> CreateAsync(string instructorId, CreateCourseDto dto);
    Task<Result<CourseDto>> UpdateAsync(Guid id, string instructorId, UpdateCourseDto dto);
    Task<Result<bool>> DeleteAsync(Guid id, string userId, bool isAdmin);
    Task<Result<bool>> TogglePublishAsync(Guid id, string instructorId);
    Task<Result<string>> UploadThumbnailAsync(Guid id, string instructorId, IFormFile file);
}

public interface IEnrollmentService
{
    Task<Result<EnrollmentDto>> EnrollAsync(string studentId, CreateEnrollmentDto dto);
    Task<Result<bool>> CancelEnrollmentAsync(Guid enrollmentId, Guid studentId);
    Task<Result<EnrollmentDto>> UpdateProgressAsync(Guid enrollmentId, string studentId, UpdateProgressDto dto);
    Task<Result<List<EnrollmentDto>>> GetMyEnrollmentsAsync(string studentId);
    Task<Result<PagedResult<EnrollmentDto>>> GetAllAsync(int page, int pageSize);
    Task<Result<ReviewDto>> AddReviewAsync(Guid courseId, Guid studentId, CreateReviewDto dto);
    Task<Result<ReviewDto>> UpdateReviewAsync(Guid reviewId, Guid studentId, UpdateReviewDto dto);
    Task<Result<bool>> DeleteReviewAsync(Guid reviewId, string userId, bool isAdmin);
    Task<Result<List<ReviewDto>>> GetCourseReviewsAsync(Guid courseId);
    Task<Result<List<LessonDto>>> GetCourseLessonsAsync(Guid courseId, string userId, bool isInstructor, bool isAdmin);
}

public interface IDashboardService
{
    Task<Result<DashboardSummaryDto>> GetSummaryAsync();
    Task<Result<List<TopCourseDto>>> GetTopCoursesAsync();
    Task<Result<List<MonthlyEnrollmentDto>>> GetEnrollmentsByMonthAsync();
    Task<Result<List<CategoryRevenueDto>>> GetRevenueByCategoryAsync();
    Task<Result<List<CompletionRateDto>>> GetCompletionRateAsync();
    Task<Result<InstructorStatsDto>> GetInstructorStatsAsync(string instructorId, string requestUserId, bool isAdmin);
    Task<Result<StudentsProgressSummaryDto>> GetStudentsProgressAsync();
    Task<Result<RatingsDistributionDto>> GetRatingsDistributionAsync();
}

public interface IEmailService
{
    Task SendAsync(EmailMessageDto message);
}

public interface IFileService
{
    Task<string?> UploadAsync(IFormFile file, string folderName);
    Task<bool> DeleteAsync(string filePath);
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken token = default);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, CancellationToken token = default);
    Task RemoveAsync(string key, CancellationToken token = default);
}
