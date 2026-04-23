# 📚 Практическая задача — Online Course Platform (Clean Architecture)

## 🎯 Цель

Разработать **REST API** для платформы онлайн-курсов на **ASP.NET Core** с использованием **Clean Architecture**.  
Система должна поддерживать управление курсами, уроками, студентами и аналитику платформы.

---

## 🗂️ Структура проекта (Clean Architecture)

```
OnlineCourses/
├── Domain/
│   ├── Entities/
│   │   ├── Course.cs
│   │   ├── Lesson.cs
│   │   ├── Category.cs
│   │   ├── Student.cs
│   │   ├── Enrollment.cs
│   │   └── Review.cs
│   └── Enums/
│       ├── CourseLevel.cs        // Beginner, Intermediate, Advanced
│       └── EnrollmentStatus.cs   // Active, Completed, Cancelled
│
├── Application/
│   ├── Common/
│   │   ├── Result.cs
│   │   └── PagedResult.cs
│   ├── DTOs/
│   │   ├── AuthDTOs/
│   │   ├── CourseDTOs/
│   │   ├── LessonDTOs/
│   │   ├── EnrollmentDTOs/
│   │   └── DashboardDto.cs
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   │   ├── ICourseRepository.cs
│   │   │   ├── ILessonRepository.cs
│   │   │   ├── IEnrollmentRepository.cs
│   │   │   └── IStudentRepository.cs
│   │   └── Services/
│   │       ├── ICourseService.cs
│   │       ├── IEnrollmentService.cs
│   │       ├── IDashboardService.cs
│   │       ├── IEmailService.cs
│   │       └── IFileService.cs
│   └── Services/
│       ├── CourseService.cs
│       ├── EnrollmentService.cs
│       ├── DashboardService.cs
│       └── FileService.cs
│
├── Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/    // Fluent API конфиги
│   ├── Repositories/
│   ├── Migrations/
│   └── Services/
│       ├── CacheService.cs
│       └── EmailService.cs       // реализация IEmailService через SMTP
│
└── WebApi/
    ├── Controllers/
    ├── Middleware/
    ├── Program.cs
    └── appsettings.json
```

---

## 🗃️ Сущности (Domain Entities)

### Course

```csharp
public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? ThumbnailPath { get; set; }    // путь к загруженному файлу (FormFile)
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public string InstructorId { get; set; }      // User.Id из Identity
    public ApplicationUser Instructor { get; set; }

    public ICollection<Lesson> Lessons { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
```

### Enrollment

```csharp
public class Enrollment
{
    public Guid Id { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public EnrollmentStatus Status { get; set; }
    public int ProgressPercent { get; set; }      // 0–100

    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    public string StudentId { get; set; }
    public ApplicationUser Student { get; set; }
}
```

### Review

```csharp
public class Review
{
    public Guid Id { get; set; }
    public int Rating { get; set; }              // 1–5
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid CourseId { get; set; }
    public string StudentId { get; set; }
}
```

---

## 🔑 Роли пользователей

| Роль | Описание |
|------|----------|
| `Admin` | Полный доступ, управление пользователями и платформой |
| `Instructor` | Создание и управление своими курсами |
| `Student` | Запись на курсы, просмотр уроков, отзывы |

```csharp
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Instructor = "Instructor";
    public const string Student = "Student";
}
```

---

## 🔐 AuthController — `/api/auth`

| Метод | Endpoint | Описание |
|-------|----------|----------|
| POST | `/register` | Регистрация (указать роль: Student или Instructor) |
| POST | `/login` | Получение JWT токена |
| POST | `/change-password` | `[Authorize]` — смена пароля |
| GET | `/me` | `[Authorize]` — текущий пользователь |
| POST | `/forgot-password` | Отправить письмо со ссылкой для сброса пароля |
| POST | `/reset-password` | Сбросить пароль по токену из письма |

**RegisterDto:**

```csharp
public class RegisterDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }   // "Student" или "Instructor"
}
```

---

## 📧 Forgot Password / Reset Password (Email Flow)

### Общий поток

```
1. POST /api/auth/forgot-password   { email }
          ↓
2. Генерация токена через UserManager.GeneratePasswordResetTokenAsync(user)
          ↓
3. Отправка письма на email пользователя со ссылкой:
   https://yourfrontend.com/reset-password?token=...&email=...
          ↓
4. POST /api/auth/reset-password   { email, token, newPassword }
          ↓
5. UserManager.ResetPasswordAsync(user, token, newPassword)
```

---

### 📩 POST `/api/auth/forgot-password`

```csharp
public class ForgotPasswordDto
{
    public string Email { get; set; }
}
```

```csharp
[HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
{
    var user = await _userManager.FindByEmailAsync(model.Email);

    // ⚠️ Всегда возвращать 200 OK — даже если email не найден
    // (чтобы не раскрывать информацию о существующих аккаунтах)
    if (user is null)
        return Ok(new { message = "Если такой email существует, письмо отправлено." });

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

    var resetLink = $"{_config["Frontend:BaseUrl"]}/reset-password?token={encodedToken}&email={user.Email}";

    await _emailService.SendAsync(new EmailMessageDto
    {
        To = user.Email,
        Subject = "Сброс пароля — Online Courses",
        Body = $"""
            <h2>Сброс пароля</h2>
            <p>Вы запросили сброс пароля. Нажмите на кнопку ниже:</p>
            <a href="{resetLink}" style="padding:10px 20px;background:#4F46E5;color:white;border-radius:6px;text-decoration:none;">
                Сбросить пароль
            </a>
            <p>Ссылка действительна <strong>15 минут</strong>.</p>
            <p>Если вы не запрашивали сброс — просто проигнорируйте это письмо.</p>
        """
    });

    return Ok(new { message = "Если такой email существует, письмо отправлено." });
}
```

---

### 🔓 POST `/api/auth/reset-password`

```csharp
public class ResetPasswordDto
{
    public string Email { get; set; }
    public string Token { get; set; }       // base64url-encoded токен из письма
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
```

```csharp
[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
{
    if (model.NewPassword != model.ConfirmPassword)
        return BadRequest(new { error = "Пароли не совпадают." });

    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user is null)
        return BadRequest(new { error = "Пользователь не найден." });

    // Декодировать токен обратно
    var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

    var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
    if (!result.Succeeded)
        return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

    return Ok(new { message = "Пароль успешно изменён." });
}
```

---

### 📬 IEmailService — интерфейс и реализация

**Интерфейс** (Application слой):

```csharp
// Application/Interfaces/Services/IEmailService.cs
public interface IEmailService
{
    Task SendAsync(EmailMessageDto message);
}

public class EmailMessageDto
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }          // HTML-контент
    public bool IsHtml { get; set; } = true;
}
```

**Реализация через SMTP** (Infrastructure слой):

```csharp
// Infrastructure/Services/EmailService.cs
public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;

    public EmailService(IOptions<SmtpSettings> options)
    {
        _smtp = options.Value;
    }

    public async Task SendAsync(EmailMessageDto message)
    {
        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(_smtp.From, _smtp.DisplayName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };

        mail.To.Add(message.To);
        await client.SendMailAsync(mail);
    }
}
```

**SmtpSettings** (конфиг-класс):

```csharp
public class SmtpSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string DisplayName { get; set; }
}
```

**appsettings.json** (добавить блок):

```json
"Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "your-email@gmail.com",
    "DisplayName": "Online Courses Platform"
},
"Frontend": {
    "BaseUrl": "http://localhost:3000"
}
```

**Регистрация в Program.cs:**

```csharp
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailService, EmailService>();
```

> 💡 Для тестирования можно использовать **Mailtrap** (https://mailtrap.io) — перехватывает письма в dev-окружении, ничего не уходит реальным пользователям.

---

## 📘 CoursesController — `/api/courses`

| Метод | Endpoint | Доступ | Описание |
|-------|----------|--------|----------|
| GET | `/` | Все | Список курсов с пагинацией и фильтрацией |
| GET | `/{id}` | Все | Подробности курса |
| POST | `/` | Instructor | Создать курс |
| PUT | `/{id}` | Instructor (свой) | Обновить курс |
| DELETE | `/{id}` | Admin / Instructor (свой) | Удалить курс |
| PATCH | `/{id}/publish` | Instructor (свой) | Опубликовать / снять с публикации |
| POST | `/{id}/thumbnail` | Instructor (свой) | Загрузить обложку курса (FormFile) |

### Фильтрация и пагинация

```csharp
public class CourseFilterDto
{
    public string? Search { get; set; }         // по Title / Description
    public Guid? CategoryId { get; set; }
    public CourseLevel? Level { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsPublished { get; set; }
    public string? SortBy { get; set; }         // "price", "rating", "createdAt"
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```

> **Ответ** должен возвращать `PagedResult<CourseDto>`.

---

## 📖 LessonsController — `/api/courses/{courseId}/lessons`

| Метод | Endpoint | Доступ | Описание |
|-------|----------|--------|----------|
| GET | `/` | Enrolled / Instructor / Admin | Список уроков |
| GET | `/{id}` | Enrolled / Instructor / Admin | Урок по ID |
| POST | `/` | Instructor (владелец курса) | Добавить урок |
| PUT | `/{id}` | Instructor (владелец) | Обновить урок |
| DELETE | `/{id}` | Instructor / Admin | Удалить урок |

> Студент видит уроки **только если записан** на курс (`EnrollmentStatus.Active`).

---

## 🎓 EnrollmentsController — `/api/enrollments`

| Метод | Endpoint | Доступ | Описание |
|-------|----------|--------|----------|
| POST | `/` | Student | Записаться на курс |
| DELETE | `/{id}` | Student (свой) | Отменить запись |
| PATCH | `/{id}/progress` | Student | Обновить прогресс (0–100) |
| GET | `/my` | Student | Мои записи |
| GET | `/` | Admin | Все записи с пагинацией |

---

## ⭐ ReviewsController — `/api/courses/{courseId}/reviews`

| Метод | Endpoint | Доступ | Описание |
|-------|----------|--------|----------|
| GET | `/` | Все | Список отзывов курса |
| POST | `/` | Student (Enrolled) | Оставить отзыв |
| PUT | `/{id}` | Student (свой) | Обновить отзыв |
| DELETE | `/{id}` | Admin / Student (свой) | Удалить отзыв |

> Студент может оставить **только один** отзыв на курс.

---

## 👤 UsersController — `/api/users`

`[Authorize(Roles = "Admin")]`

| Метод | Endpoint | Описание |
|-------|----------|----------|
| GET | `/` | Список пользователей с пагинацией |
| GET | `/{id}` | Пользователь по ID |
| PUT | `/{id}/role` | Изменить роль пользователя |
| DELETE | `/{id}` | Удалить пользователя |

---

## 📊 DashboardController — `/api/dashboard`

`[Authorize(Roles = "Admin")]` — если не указано иное

---

### 1️⃣ GET `/api/dashboard/summary`

Общая сводка платформы.

```csharp
public class DashboardSummaryDto
{
    public int TotalCourses { get; set; }
    public int PublishedCourses { get; set; }
    public int TotalStudents { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }    // Status == Active
    public int CompletedEnrollments { get; set; } // Status == Completed
    public decimal TotalRevenue { get; set; }
    public double AveragePlatformRating { get; set; }
    public int TotalReviews { get; set; }
}
```

> 🔴 Кэшировать в **Redis** на 5 минут (`dashboard:summary`). Инвалидировать при регистрации нового пользователя или создании курса.

---

### 2️⃣ GET `/api/dashboard/top-courses`

Топ-10 курсов по количеству записей.

```csharp
public class TopCourseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string InstructorName { get; set; }
    public int EnrollmentCount { get; set; }
    public int CompletedCount { get; set; }       // сколько завершили
    public double CompletionRate { get; set; }    // CompletedCount / EnrollmentCount * 100
    public double AverageRating { get; set; }
    public decimal Revenue { get; set; }          // EnrollmentCount * Course.Price
}
```

> Реализовать через LINQ GroupBy + проекцию. Не загружать все сущности в память.

---

### 3️⃣ GET `/api/dashboard/enrollments-by-month`

Динамика новых записей по месяцам за последние **12 месяцев**.

```csharp
public class MonthlyEnrollmentDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; }   // "Январь", "Февраль" ...
    public int NewEnrollments { get; set; }
    public int Completions { get; set; }
    public decimal Revenue { get; set; }
}
```

> Запрос должен выполняться **на уровне БД** (не в памяти):
> ```csharp
> var data = await _context.Enrollments
>     .Where(e => e.EnrolledAt >= DateTime.UtcNow.AddMonths(-12))
>     .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
>     .Select(g => new MonthlyEnrollmentDto
>     {
>         Year = g.Key.Year,
>         Month = g.Key.Month,
>         NewEnrollments = g.Count(),
>         Completions = g.Count(e => e.Status == EnrollmentStatus.Completed),
>         Revenue = g.Sum(e => e.Course.Price)
>     })
>     .OrderBy(x => x.Year).ThenBy(x => x.Month)
>     .ToListAsync();
> ```

---

### 4️⃣ GET `/api/dashboard/revenue-by-category`

Выручка и количество студентов по каждой категории.

```csharp
public class CategoryRevenueDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int CourseCount { get; set; }
    public int TotalStudents { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
}
```

> Отсортировать по `TotalRevenue DESC`. Полезно понимать какие категории приносят больше дохода.

---

### 5️⃣ GET `/api/dashboard/completion-rate`

Процент завершаемости по курсам — аналитика "насколько хорошо студенты заканчивают курсы".

```csharp
public class CompletionRateDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public int TotalEnrolled { get; set; }
    public int TotalCompleted { get; set; }
    public double CompletionRatePercent { get; set; }
    public double AverageProgressPercent { get; set; }  // среднее ProgressPercent по всем записям
}
```

> Вернуть только курсы у которых `TotalEnrolled >= 5` (чтобы исключить нерепрезентативные данные).

---

### 6️⃣ GET `/api/dashboard/instructor/{instructorId}`

`[Authorize(Roles = "Admin,Instructor")]`  
Инструктор может запрашивать **только свою** статистику. Admin — любого.

```csharp
public class InstructorStatsDto
{
    public string InstructorName { get; set; }
    public int CourseCount { get; set; }
    public int PublishedCourseCount { get; set; }
    public int TotalStudents { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<TopCourseDto> TopCourses { get; set; }          // топ-3 своих курса
    public List<MonthlyEnrollmentDto> EnrollmentTrend { get; set; }  // за 6 месяцев
}
```

---

### 7️⃣ GET `/api/dashboard/students-progress`

`[Authorize(Roles = "Admin")]`  
Сводка активности студентов на платформе.

```csharp
public class StudentsProgressSummaryDto
{
    public int TotalStudents { get; set; }
    public int StudentsWithActiveEnrollment { get; set; }
    public int StudentsCompletedAtLeastOne { get; set; }    // завершили хотя бы 1 курс
    public int StudentsNeverStarted { get; set; }           // зарегистрированы, но ни разу не записались
    public double AverageCoursesPerStudent { get; set; }
    public List<StudentProgressDto> TopActiveStudents { get; set; }  // топ-5 по количеству завершённых курсов
}

public class StudentProgressDto
{
    public string StudentId { get; set; }
    public string FullName { get; set; }
    public int CompletedCourses { get; set; }
    public int ActiveEnrollments { get; set; }
    public double AverageProgress { get; set; }
}
```

---

### 8️⃣ GET `/api/dashboard/ratings-distribution`

Распределение оценок по всей платформе.

```csharp
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
```

> ```csharp
> var distribution = await _context.Reviews
>     .GroupBy(r => r.Rating)
>     .Select(g => new { Rating = g.Key, Count = g.Count() })
>     .ToListAsync();
> ```

---

### Сводная таблица аналитических эндпоинтов

| Endpoint | Доступ | Кэш | Описание |
|----------|--------|-----|----------|
| `GET /summary` | Admin | Redis 5 мин | Общие показатели платформы |
| `GET /top-courses` | Admin | Redis 30 мин | Топ-10 курсов по записям + доход |
| `GET /enrollments-by-month` | Admin | — | Динамика записей за 12 мес. |
| `GET /revenue-by-category` | Admin | Redis 15 мин | Доход по категориям |
| `GET /completion-rate` | Admin | — | Завершаемость по курсам |
| `GET /instructor/{id}` | Admin + Instructor (свой) | — | Статистика инструктора |
| `GET /students-progress` | Admin | — | Активность студентов |
| `GET /ratings-distribution` | Admin | Redis 15 мин | Распределение оценок |

---

## 🖼️ Загрузка файлов (FormFile)

```csharp
// POST /api/courses/{id}/thumbnail
[Authorize(Roles = "Instructor")]
[HttpPost("{id}/thumbnail")]
public async Task<IActionResult> UploadThumbnail(Guid id, IFormFile file)
{
    // Валидация: только image/jpeg, image/png — до 5MB
    // Сохранить в wwwroot/uploads/thumbnails/
    // Вернуть публичный URL
}
```

**Требования к валидации файла:**
- Разрешённые типы: `image/jpeg`, `image/png`
- Максимальный размер: **5 MB**
- Генерировать уникальное имя: `{courseId}_{timestamp}.{ext}`

---

## ⚙️ EF Core + Fluent API

```csharp
// CourseConfiguration.cs
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(c => c.Category)
            .WithMany(cat => cat.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Enrollments)
            .WithOne(e => e.Course)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Уникальный индекс — один студент, один курс
        builder.HasIndex(nameof(Enrollment.StudentId), nameof(Enrollment.CourseId))
            .IsUnique();
    }
}
```

---

## 🔁 Result Pattern

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public int StatusCode { get; }

    public static Result<T> Success(T value) => new(true, value, null, 200);
    public static Result<T> Failure(string error, int statusCode = 400) => new(false, default, error, statusCode);
    public static Result<T> NotFound(string error) => new(false, default, error, 404);
    public static Result<T> Forbidden(string error) => new(false, default, error, 403);
}
```

> Все методы сервисов **обязаны** возвращать `Result<T>`. Контроллер преобразует результат в `IActionResult`.

---

## 🚀 Кэширование (Memory Cache + Redis)

### Memory Cache

```csharp
// Кэшировать список категорий на 10 минут
public async Task<Result<List<CategoryDto>>> GetCategoriesAsync()
{
    const string key = "categories_all";
    if (_memoryCache.TryGetValue(key, out List<CategoryDto>? cached))
        return Result<List<CategoryDto>>.Success(cached!);

    var categories = await _categoryRepository.GetAllAsync();
    _memoryCache.Set(key, categories, TimeSpan.FromMinutes(10));
    return Result<List<CategoryDto>>.Success(categories);
}
```

### Redis

```csharp
// Кэшировать топ-5 курсов в Redis на 30 минут
public async Task<Result<List<TopCourseDto>>> GetTopCoursesAsync()
{
    const string key = "dashboard:top_courses";
    var cached = await _redis.GetStringAsync(key);

    if (cached is not null)
        return Result<List<TopCourseDto>>.Success(JsonSerializer.Deserialize<List<TopCourseDto>>(cached)!);

    var top = await _enrollmentRepository.GetTopCoursesAsync(5);
    await _redis.SetStringAsync(key, JsonSerializer.Serialize(top), new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    });

    return Result<List<TopCourseDto>>.Success(top);
}
```

> Инвалидировать Redis-ключ `dashboard:top_courses` при каждой новой записи на курс.

---

## 🌐 CORS

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://mycoursesapp.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

---

## 🛡️ Middleware — Exception Handling

```csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Логировать ошибку
            // Вернуть унифицированный JSON-ответ
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Internal server error",
                message = ex.Message
            });
        }
    }
}
```

---

## 🧪 Тестовые сценарии

| Сценарий | Ожидаемый результат |
|----------|---------------------|
| Student пытается создать курс | ❌ 403 Forbidden |
| Instructor пытается удалить **чужой** курс | ❌ 403 Forbidden |
| Student записывается на курс дважды | ❌ 400 Bad Request |
| Student оставляет отзыв без записи | ❌ 400 Bad Request |
| Запрос без токена к `/api/dashboard` | ❌ 401 Unauthorized |
| Admin получает `/api/dashboard/summary` | ✅ 200 OK + данные |
| Instructor видит свою статистику | ✅ 200 OK |
| Instructor пытается получить статистику **чужого** instructorId | ❌ 403 Forbidden |
| Загрузка файла > 5MB | ❌ 400 Bad Request |
| Загрузка `.pdf` как thumbnail | ❌ 400 Bad Request |
| Получение топ-курсов (второй раз — из кэша) | ✅ 200 OK + быстрее |
| `POST /forgot-password` с несуществующим email | ✅ 200 OK (без раскрытия информации) |
| `POST /forgot-password` с валидным email | ✅ 200 OK + письмо отправлено |
| `POST /reset-password` с верным токеном | ✅ 200 OK + пароль изменён |
| `POST /reset-password` с просроченным/неверным токеном | ❌ 400 Bad Request |
| `POST /reset-password` с несовпадающими паролями | ❌ 400 Bad Request |
| `GET /dashboard/completion-rate` с курсом < 5 студентов | ✅ 200 OK, курс исключён из списка |
| `GET /dashboard/ratings-distribution` | ✅ 200 OK + корректный подсчёт по 1–5 звёздам |

---

## 📌 Инициализация ролей и admin-пользователя при старте

```csharp
// DbSeeder.cs
public static async Task SeedAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { UserRoles.Admin, UserRoles.Instructor, UserRoles.Student };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Создать дефолтного Admin если не существует
    if (await userManager.FindByEmailAsync("admin@courses.com") is null)
    {
        var admin = new ApplicationUser { UserName = "admin", Email = "admin@courses.com" };
        await userManager.CreateAsync(admin, "Admin@123!");
        await userManager.AddToRoleAsync(admin, UserRoles.Admin);
    }
}
```

---

## ⚙️ appsettings.json (структура)

```json
{
  "ConnectionStrings": {
    "Default": "Server=...;Database=OnlineCoursesDb;..."
  },
  "Jwt": {
    "Key": "your-super-secret-key-32-chars-min",
    "Issuer": "OnlineCoursesAPI",
    "Audience": "OnlineCoursesClient",
    "ExpiresInMinutes": 60
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "FileStorage": {
    "UploadPath": "wwwroot/uploads",
    "MaxFileSizeBytes": 5242880
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:3000" ]
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "your-email@gmail.com",
    "DisplayName": "Online Courses Platform"
  },
  "Frontend": {
    "BaseUrl": "http://localhost:3000"
  }
}
```

---

## 📌 Рекомендации по реализации

- Все ID использовать как `Guid`, генерировать на уровне базы данных
- Применить **AutoMapper** или **Manual Mapping** для DTO-преобразований
- Все репозитории реализовывать через **интерфейсы** (DI)
- Инструктор может редактировать / удалять **только свои** курсы — проверять `InstructorId == currentUserId`
- Пагинация: минимальный `PageSize = 1`, максимальный `PageSize = 50`
- Логировать запросы через встроенный `ILogger<T>`
- При удалении курса — **каскадно** удалять уроки и записи (через EF Core)

---

## 🏆 Бонусное задание (по желанию)

- [ ] Добавить **Permission-Based Authorization** (например: `course.publish`, `course.delete`)  
- [ ] Реализовать **Refresh Token** (хранить в БД, endpoint `/api/auth/refresh`)  
- [ ] Добавить **Background Job** (Hangfire или BackgroundService): каждую ночь отправлять email студентам с прогрессом  
- [ ] Добавить **Rate Limiting** на эндпоинт `/api/auth/login`
