using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NewMvcApp.Models;

namespace NewMvcApp.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _context;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ApiService(HttpClient http, IHttpContextAccessor context)
    {
        _http = http;
        _context = context;
    }

    private void SetAuth()
    {
        var token = _context.HttpContext?.Session.GetString("jwt");
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            _http.DefaultRequestHeaders.Authorization = null;
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        SetAuth();
        var resp = await _http.GetAsync(url);
        if (!resp.IsSuccessStatusCode) return default;
        var json = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _json);
    }

    private async Task<T?> PostAsync<T>(string url, object body)
    {
        SetAuth();
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync(url, content);
        var json = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _json);
    }

    private async Task<T?> PutAsync<T>(string url, object body)
    {
        SetAuth();
        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var resp = await _http.PutAsync(url, content);
        if (!resp.IsSuccessStatusCode) return default;
        var json = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, _json);
    }

    private async Task<bool> DeleteAsync(string url)
    {
        SetAuth();
        var resp = await _http.DeleteAsync(url);
        return resp.IsSuccessStatusCode;
    }
    
    public async Task<ApiResult<LoginResponse>?> LoginAsync(LoginDto dto)
        => await PostAsync<ApiResult<LoginResponse>>("api/Auth/login", dto);

    public async Task<ApiResult<object>?> RegisterAsync(RegisterDto dto)
        => await PostAsync<ApiResult<object>>("api/Auth/register", dto);
    
    public async Task<ApiResult<List<CourseDto>>?> GetCoursesAsync()
        => await GetAsync<ApiResult<List<CourseDto>>>("api/Course");

    public async Task<ApiResult<CourseDto>?> GetCourseAsync(Guid id)
        => await GetAsync<ApiResult<CourseDto>>($"api/Course/{id}");

    public async Task<ApiResult<object>?> CreateCourseAsync(CreateCourseDto dto)
        => await PostAsync<ApiResult<object>>("api/Course", dto);

    public async Task<bool> DeleteCourseAsync(Guid id)
        => await DeleteAsync($"api/Course/{id}");

    public async Task<ApiResult<object>?> TogglePublishAsync(Guid id)
        => await PutAsync<ApiResult<object>>($"api/Course/{id}/publish", new {});
    
    public async Task<ApiResult<List<CategoryDto>>?> GetCategoriesAsync()
        => await GetAsync<ApiResult<List<CategoryDto>>>("api/Category");

    public async Task<ApiResult<object>?> CreateCategoryAsync(CreateCategoryDto dto)
        => await PostAsync<ApiResult<object>>("api/Category", dto);

    public async Task<bool> DeleteCategoryAsync(Guid id)
        => await DeleteAsync($"api/Category/{id}");
    
    public async Task<ApiResult<List<LessonDto>>?> GetLessonsAsync(Guid courseId)
        => await GetAsync<ApiResult<List<LessonDto>>>($"api/Lesson/course/{courseId}");
    
    public async Task<ApiResult<List<EnrollmentDto>>?> GetMyEnrollmentsAsync()
    => await GetAsync<ApiResult<List<EnrollmentDto>>>("api/Enrollment/my");

    public async Task<ApiResult<object>?> EnrollAsync(Guid courseId)
    => await PostAsync<ApiResult<object>>("api/Enrollment", new { courseId });

    public async Task<bool> CancelEnrollmentAsync(Guid enrollmentId)
    => await DeleteAsync($"api/Enrollment/{enrollmentId}");

    public async Task<ApiResult<object>?> UpdateProgressAsync(Guid enrollmentId, int progressPercent)
    => await PutAsync<ApiResult<object>>($"api/Enrollment/{enrollmentId}/progress", new { progressPercent });
    
    public async Task<ApiResult<List<ReviewDto>>?> GetReviewsAsync(Guid courseId)
    => await GetAsync<ApiResult<List<ReviewDto>>>($"api/Review/course/{courseId}");

    public async Task<ApiResult<object>?> CreateReviewAsync(Guid courseId, CreateReviewDto dto)
    => await PostAsync<ApiResult<object>>($"api/Review/course/{courseId}", dto);
    
    public async Task<ApiResult<List<StudentDto>>?> GetStudentsAsync()
    => await GetAsync<ApiResult<List<StudentDto>>>("api/Student");

    public async Task<ApiResult<StudentDto>?> GetStudentAsync(string id)
    => await GetAsync<ApiResult<StudentDto>>($"api/Student/{id}");

    public async Task<ApiResult<object>?> UpdateStudentAsync(string id, UpdateStudentDto dto)
    => await PutAsync<ApiResult<object>>($"api/Student/{id}", dto);

    public async Task<bool> DeleteStudentAsync(string id)
    => await DeleteAsync($"api/Student/{id}");

    public async Task<ApiResult<LessonDto>?> GetLessonAsync(Guid id)
    => await GetAsync<ApiResult<LessonDto>>($"api/Lesson/{id}");

    public async Task<ApiResult<object>?> CreateLessonAsync(Guid courseId, CreateLessonDto dto)
    => await PostAsync<ApiResult<object>>($"api/Lesson/course/{courseId}", dto);

    public async Task<ApiResult<object>?> UpdateLessonAsync(Guid id, UpdateLessonDto dto)
    => await PutAsync<ApiResult<object>>($"api/Lesson/{id}", dto);

    public async Task<bool> DeleteLessonAsync(Guid id)
    => await DeleteAsync($"api/Lesson/{id}");
}
