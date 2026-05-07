using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRazorApp.Models;
using NewRazorApp.Services;

namespace NewRazorApp.Pages.Instructors;

public class IndexModel : PageModel
{
    private readonly ApiService _api;
    public List<StudentDto> Instructors { get; set; } = new();

    public IndexModel(ApiService api) => _api = api;

    public async Task OnGetAsync()
    {
        // Инструкторы — студенты с ролью Instructor
        // Получаем всех пользователей через admin endpoint
        var result = await _api.GetStudentsAsync();
        Instructors = result?.Data?.Where(s => s.Role == "Instructor").ToList() ?? new();
    }
}