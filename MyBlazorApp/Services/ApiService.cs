using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MyBlazorApp.Models;

namespace MyBlazorApp.Services;

public class ApiService
{
    private readonly HttpClient _http;

    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string? Token { get; private set; }
    public LoginResponse? CurrentUser { get; private set; }

    public event Action? OnChange;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    #region Helpers

    private void SetAuth()
    {
        _http.DefaultRequestHeaders.Authorization =
            !string.IsNullOrWhiteSpace(Token)
                ? new AuthenticationHeaderValue("Bearer", Token)
                : null;
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            SetAuth();
            return await _http.GetFromJsonAsync<T>(url, _json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GET ERROR [{url}] => {ex.Message}");
            return default;
        }
    }

    private async Task<T?> PostAsync<T>(string url, object body)
    {
        try
        {
            SetAuth();
            var response = await _http.PostAsJsonAsync(url, body);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"POST ERROR [{url}] => {ex.Message}");
            return default;
        }
    }

    private async Task<T?> PutAsync<T>(string url, object body)
    {
        try
        {
            SetAuth();
            var response = await _http.PutAsJsonAsync(url, body);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PUT ERROR [{url}] => {ex.Message}");
            return default;
        }
    }

    private async Task<T?> PatchAsync<T>(string url, object body)
    {
        try
        {
            SetAuth();
            var content = JsonContent.Create(body);
            var response = await _http.PatchAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PATCH ERROR [{url}] => {ex.Message}");
            return default;
        }
    }

    private async Task<bool> DeleteAsync(string url)
    {
        try
        {
            SetAuth();
            var response = await _http.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DELETE ERROR [{url}] => {ex.Message}");
            return false;
        }
    }

    private async Task<T?> PostFormAsync<T>(string url, MultipartFormDataContent form)
    {
        try
        {
            SetAuth();
            var response = await _http.PostAsync(url, form);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"POST-FORM ERROR [{url}] => {ex.Message}");
            return default;
        }
    }

    private async Task<T?> PutFormAsync<T>(string url, MultipartFormDataContent form)
    {
        try
        {
            SetAuth();
            var response = await _http.PutAsync(url, form);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PUT-FORM ERROR [{url}] => {ex.Message}");
            return default;
        }
    }

    #endregion

    // =========================================================
    // AUTH  →  api/Auth/...
    // =========================================================

    public async Task<(bool ok, string? err)> LoginAsync(LoginDto dto)
    {
        var response = await PostAsync<ApiResult<LoginResponse>>("api/Auth/login", dto);
        if (response?.IsSuccess == true && response.Data != null)
        {
            Token = response.Data.Token;
            CurrentUser = response.Data;
            OnChange?.Invoke();
            return (true, null);
        }
        return (false, response?.Error ?? "Ошибка входа");
    }

    public async Task<(bool ok, string? err)> RegisterAsync(RegisterDto dto)
    {
        var response = await PostAsync<ApiResult<RegisterResponseDto>>("api/Auth/register", dto);
        return (response?.IsSuccess == true, response?.Error);
    }

    public async Task<(bool ok, string? err)> SendEmailAsync(SendEmailDto dto)
    {
        var response = await PostAsync<ApiResult<object>>("api/Auth/send-email", dto);
        return (response?.IsSuccess == true, response?.Error);
    }

    public async Task<(bool ok, string? err)> VerifyCodeAsync(VerifyCodeDto dto)
    {
        var response = await PostAsync<ApiResult<object>>("api/Auth/verify-code", dto);
        return (response?.IsSuccess == true, response?.Error);
    }

    public async Task<(bool ok, string? err)> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var response = await PostAsync<ApiResult<object>>("api/Auth/reset-password", dto);
        return (response?.IsSuccess == true, response?.Error);
    }

    public async Task<(bool ok, string? err)> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var response = await PutAsync<ApiResult<object>>("api/Auth/change-password", dto);
        return (response?.IsSuccess == true, response?.Error);
    }

    /// <summary>POST api/Auth/assign-role/{id}  (Admin only)</summary>
    public async Task<(bool ok, string? err)> AssignRoleAsync(string userId, string role)
    {
        var response = await PostAsync<ApiResult<object>>($"api/Auth/assign-role/{userId}?role={Uri.EscapeDataString(role)}", new { });
        return (response?.IsSuccess == true, response?.Error);
    }

    public void Logout()
    {
        Token = null;
        CurrentUser = null;
        OnChange?.Invoke();
    }

    // =========================================================
    // COURSES  →  api/Course/...
    // =========================================================

    /// <summary>GET api/Course</summary>
    public Task<ApiResult<List<GetCourseDto>>?> GetCoursesAsync()
        => GetAsync<ApiResult<List<GetCourseDto>>>("api/Course");

    // Псевдоним, который используют страницы Blazor
    public Task<ApiResult<List<GetCourseDto>>?> GetCourses()
        => GetCoursesAsync();

    /// <summary>GET api/Course/{id}</summary>
    public Task<ApiResult<GetCourseDto>?> GetCourseByIdAsync(Guid id)
        => GetAsync<ApiResult<GetCourseDto>>($"api/Course/{id}");

    // Псевдоним, который используют страницы Blazor
    public Task<ApiResult<GetCourseDto>?> GetCourse(Guid id)
        => GetCourseByIdAsync(id);

    /// <summary>POST api/Course</summary>
    public Task<ApiResult<CreateCourseResponseDto>?> CreateCourseAsync(CreateCourseDto dto)
        => PostAsync<ApiResult<CreateCourseResponseDto>>("api/Course", dto);

    /// <summary>PUT api/Course/{id}</summary>
    public Task<ApiResult<UpdateCourseResponseDto>?> UpdateCourseAsync(Guid id, UpdateCourseDto dto)
        => PutAsync<ApiResult<UpdateCourseResponseDto>>($"api/Course/{id}", dto);

    /// <summary>DELETE api/Course/{id}</summary>
    public Task<bool> DeleteCourseAsync(Guid id)
        => DeleteAsync($"api/Course/{id}");

    /// <summary>PATCH api/Course/{id}/publish  (был PUT — исправлен на PATCH)</summary>
    public Task<ApiResult<object>?> TogglePublishAsync(Guid id)
        => PatchAsync<ApiResult<object>>($"api/Course/{id}/publish", new { });

    /// <summary>POST api/Course/{id}/thumbnail  (multipart/form-data)</summary>
    public Task<ApiResult<object>?> UploadThumbnailAsync(Guid id, Stream fileStream, string fileName, string contentType = "image/jpeg")
    {
        var form = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(streamContent, "file", fileName);
        return PostFormAsync<ApiResult<object>>($"api/Course/{id}/thumbnail", form);
    }

    // =========================================================
    // CATEGORIES  →  api/Category/...
    // =========================================================

    /// <summary>GET api/Category</summary>
    public Task<ApiResult<List<GetCategoryDto>>?> GetCategoriesAsync()
        => GetAsync<ApiResult<List<GetCategoryDto>>>("api/Category");

    // Псевдоним, который используют страницы Blazor
    public Task<ApiResult<List<GetCategoryDto>>?> GetCategories()
        => GetCategoriesAsync();

    /// <summary>GET api/Category/{id}</summary>
    public Task<ApiResult<GetCategoryDto>?> GetCategoryByIdAsync(Guid id)
        => GetAsync<ApiResult<GetCategoryDto>>($"api/Category/{id}");

    /// <summary>POST api/Category</summary>
    public Task<ApiResult<CreateCategoryResponseDto>?> CreateCategoryAsync(CreateCategoryDto dto)
        => PostAsync<ApiResult<CreateCategoryResponseDto>>("api/Category", dto);

    /// <summary>PUT api/Category/{id}</summary>
    public Task<ApiResult<CreateCategoryResponseDto>?> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        => PutAsync<ApiResult<CreateCategoryResponseDto>>($"api/Category/{id}", dto);

    /// <summary>DELETE api/Category/{id}</summary>
    public Task<bool> DeleteCategoryAsync(Guid id)
        => DeleteAsync($"api/Category/{id}");

    // =========================================================
    // LESSONS  →  api/courses/{courseId}/lessons/...
    // =========================================================

    /// <summary>GET api/courses/{courseId}/lessons</summary>
    public Task<ApiResult<List<GetLessonDto>>?> GetLessonsByCourseAsync(Guid courseId)
        => GetAsync<ApiResult<List<GetLessonDto>>>($"api/courses/{courseId}/lessons");

    // Псевдоним, который используют страницы Blazor
    public Task<ApiResult<List<GetLessonDto>>?> GetLessons(Guid courseId)
        => GetLessonsByCourseAsync(courseId);

    /// <summary>GET api/courses/{courseId}/lessons/{id}</summary>
    public Task<ApiResult<GetLessonDto>?> GetLessonByIdAsync(Guid courseId, Guid id)
        => GetAsync<ApiResult<GetLessonDto>>($"api/courses/{courseId}/lessons/{id}");

    /// <summary>POST api/courses/{courseId}/lessons</summary>
    public Task<ApiResult<CreateLessonResponseDto>?> CreateLessonAsync(Guid courseId, CreateLessonDto dto)
        => PostAsync<ApiResult<CreateLessonResponseDto>>($"api/courses/{courseId}/lessons", dto);

    // Псевдоним, который используют страницы Blazor (CourseId берётся из dto)
    public Task<ApiResult<CreateLessonResponseDto>?> CreateLesson(CreateLessonDto dto)
        => CreateLessonAsync(dto.CourseId, dto);

    /// <summary>PUT api/courses/{courseId}/lessons/{id}</summary>
    public Task<ApiResult<UpdateLessonResponseDto>?> UpdateLessonAsync(Guid courseId, Guid id, UpdateLessonDto dto)
        => PutAsync<ApiResult<UpdateLessonResponseDto>>($"api/courses/{courseId}/lessons/{id}", dto);

    /// <summary>DELETE api/courses/{courseId}/lessons/{id}</summary>
    public Task<bool> DeleteLessonAsync(Guid courseId, Guid id)
        => DeleteAsync($"api/courses/{courseId}/lessons/{id}");

    // Псевдоним, который используют страницы Blazor — courseId неизвестен,
    // поэтому нужен перегруженный вариант или хранить маппинг lessonId→courseId.
    // Здесь делаем перегрузку через отдельный метод:
    public Task<bool> DeleteLesson(Guid courseId, Guid lessonId)
        => DeleteLessonAsync(courseId, lessonId);

    // =========================================================
    // ENROLLMENTS  →  api/enrollments/...
    // =========================================================

    /// <summary>GET api/enrollments  (Admin/Instructor)</summary>
    public Task<ApiResult<List<GetEnrollmentDto>>?> GetEnrollmentsAsync()
        => GetAsync<ApiResult<List<GetEnrollmentDto>>>("api/enrollments");

    /// <summary>GET api/enrollments/my  (текущий пользователь)</summary>
    public Task<ApiResult<List<GetEnrollmentDto>>?> GetMyEnrollmentsAsync()
        => GetAsync<ApiResult<List<GetEnrollmentDto>>>("api/enrollments/my");

    /// <summary>POST api/enrollments</summary>
    public Task<ApiResult<CreateEnrollmentResponseDto>?> CreateEnrollmentAsync(CreateEnrollmentDto dto)
        => PostAsync<ApiResult<CreateEnrollmentResponseDto>>("api/enrollments", dto);

    /// <summary>PATCH api/enrollments/{id}/progress</summary>
    public Task<ApiResult<GetEnrollmentDto>?> UpdateProgressAsync(Guid id, UpdateEnrollmentDto dto)
        => PatchAsync<ApiResult<GetEnrollmentDto>>($"api/enrollments/{id}/progress", dto);

    /// <summary>DELETE api/enrollments/{id}</summary>
    public Task<bool> DeleteEnrollmentAsync(Guid id)
        => DeleteAsync($"api/enrollments/{id}");

    // =========================================================
    // REVIEWS  →  api/courses/{courseId}/reviews/...
    // =========================================================

    /// <summary>GET api/courses/{courseId}/reviews</summary>
    public Task<ApiResult<List<GetReviewDto>>?> GetCourseReviewsAsync(Guid courseId)
        => GetAsync<ApiResult<List<GetReviewDto>>>($"api/courses/{courseId}/reviews");

    /// <summary>POST api/courses/{courseId}/reviews</summary>
    public Task<ApiResult<CreateReviewResponseDto>?> CreateReviewAsync(Guid courseId, CreateReviewDto dto)
        => PostAsync<ApiResult<CreateReviewResponseDto>>($"api/courses/{courseId}/reviews", dto);

    /// <summary>PUT api/courses/{courseId}/reviews/{id}</summary>
    public Task<ApiResult<UpdateReviewResponseDto>?> UpdateReviewAsync(Guid courseId, Guid id, UpdateReviewDto dto)
        => PutAsync<ApiResult<UpdateReviewResponseDto>>($"api/courses/{courseId}/reviews/{id}", dto);

    /// <summary>DELETE api/courses/{courseId}/reviews/{id}</summary>
    public Task<bool> DeleteReviewAsync(Guid courseId, Guid id)
        => DeleteAsync($"api/courses/{courseId}/reviews/{id}");

    // =========================================================
    // STUDENTS  →  api/students/...
    // =========================================================

    /// <summary>GET api/students  (Admin/Instructor)</summary>
    public Task<ApiResult<List<GetStudentDto>>?> GetStudentsAsync()
        => GetAsync<ApiResult<List<GetStudentDto>>>("api/students");

    // Псевдоним, который используют страницы Blazor
    public Task<ApiResult<List<GetStudentDto>>?> GetUsers()
        => GetStudentsAsync();

    /// <summary>GET api/students/{id}</summary>
    public Task<ApiResult<GetStudentDto>?> GetStudentByIdAsync(string id)
        => GetAsync<ApiResult<GetStudentDto>>($"api/students/{id}");

    /// <summary>PUT api/students/{id}</summary>
    public Task<ApiResult<UpdateStudentResponseDto>?> UpdateStudentAsync(string id, UpdateStudentDto dto)
        => PutAsync<ApiResult<UpdateStudentResponseDto>>($"api/students/{id}", dto);

    /// <summary>DELETE api/students/{id}  (Admin/Instructor)</summary>
    public Task<bool> DeleteStudentAsync(string id)
        => DeleteAsync($"api/students/{id}");

    /// <summary>PUT api/students/change-avatar/{id}  (multipart/form-data)</summary>
    public Task<ApiResult<object>?> UploadAvatarAsync(string id, Stream fileStream, string fileName, string contentType = "image/jpeg")
    {
        var form = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(streamContent, "file", fileName);
        return PutFormAsync<ApiResult<object>>($"api/students/change-avatar/{id}", form);
    }

    /// <summary>DELETE api/students/delete-avatar/{id}</summary>
    public Task<bool> DeleteAvatarAsync(string id)
        => DeleteAsync($"api/students/delete-avatar/{id}");

    // =========================================================
    // DASHBOARD  →  api/dashboards/...  (Admin only)
    // =========================================================

    /// <summary>GET api/dashboards/summary</summary>
    public Task<ApiResult<DashboardSummaryDto>?> GetDashboardSummaryAsync()
        => GetAsync<ApiResult<DashboardSummaryDto>>("api/dashboards/summary");

    /// <summary>GET api/dashboards/top-courses</summary>
    public Task<ApiResult<List<TopCourseDto>>?> GetTopCoursesAsync()
        => GetAsync<ApiResult<List<TopCourseDto>>>("api/dashboards/top-courses");

    /// <summary>GET api/dashboards/enrollments-by-month</summary>
    public Task<ApiResult<List<MonthlyEnrollmentDto>>?> GetMonthlyEnrollmentsAsync()
        => GetAsync<ApiResult<List<MonthlyEnrollmentDto>>>("api/dashboards/enrollments-by-month");

    // =========================================================
    // HELPERS / ROLE CHECKS
    // =========================================================

    public bool IsAdmin =>
        CurrentUser?.Role == "Admin";

    public bool IsInstructor =>
        CurrentUser?.Role == "Instructor" || IsAdmin;

    public bool IsStudent =>
        CurrentUser?.Role == "Student";

    public bool IsLoggedIn =>
        CurrentUser != null && !string.IsNullOrWhiteSpace(Token);

    public List<GetCourseDto> FilterMyCourses(List<GetCourseDto> courses)
    {
        if (CurrentUser == null) return courses;
        return courses
            .Where(x => x.InstructorName == CurrentUser.FullName)
            .ToList();
    }
    
    public async Task<ResultDto> UpdateUserRoleAsync(string userId, string newRole)
    {
        // Отправляем PATCH или POST запрос на ваш контроллер
        var response = await _http.PostAsJsonAsync($"api/admin/users/{userId}/role", new { Role = newRole });
        return await response.Content.ReadFromJsonAsync<ResultDto>();
    }
}
