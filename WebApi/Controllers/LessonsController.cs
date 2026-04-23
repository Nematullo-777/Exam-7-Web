using Application.DTOs.LessonDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[Route("api/courses/{courseId:guid}/lessons")]
[ApiController]
[Authorize]
public class LessonsController(
    IEnrollmentService enrollmentService,
    ILessonRepository lessonRepository,
    ICourseRepository courseRepository) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool IsInstructor => User.IsInRole(UserRoles.Instructor);
    private bool IsAdmin => User.IsInRole(UserRoles.Admin);

    [HttpGet]
    public async Task<IActionResult> GetAll(Guid courseId)
    {
        var result = await enrollmentService.GetCourseLessonsAsync(courseId, UserId, IsInstructor, IsAdmin);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid courseId, Guid id)
    {
        var lesson = await lessonRepository.GetByIdAsync(id);
        if (lesson is null || lesson.CourseId != courseId)
            return NotFound(new { error = "Lesson not found." });

        return Ok(new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Content = lesson.Content,
            VideoUrl = lesson.VideoUrl,
            Order = lesson.Order,
            DurationMinutes = lesson.DurationMinutes,
            CourseId = lesson.CourseId
        });
    }

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPost]
    public async Task<IActionResult> Create(Guid courseId, [FromBody] CreateLessonDto dto)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course is null) return NotFound(new { error = "Course not found." });
        if (course.InstructorId != UserId) return Forbid();

        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = dto.Title,
            Content = dto.Content,
            VideoUrl = dto.VideoUrl,
            Order = dto.Order,
            DurationMinutes = dto.DurationMinutes,
            CreatedAt = DateTime.UtcNow
        };

        var created = await lessonRepository.CreateAsync(lesson);
        return CreatedAtAction(nameof(GetById), new { courseId, id = created.Id }, new LessonDto
        {
            Id = created.Id,
            Title = created.Title,
            Content = created.Content,
            VideoUrl = created.VideoUrl,
            Order = created.Order,
            DurationMinutes = created.DurationMinutes,
            CourseId = created.CourseId
        });
    }

    [Authorize(Roles = UserRoles.Instructor)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid courseId, Guid id, [FromBody] UpdateLessonDto dto)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course is null) return NotFound(new { error = "Course not found." });
        if (course.InstructorId != UserId) return Forbid();

        var lesson = await lessonRepository.GetByIdAsync(id);
        if (lesson is null || lesson.CourseId != courseId) return NotFound(new { error = "Lesson not found." });

        lesson.Title = dto.Title;
        lesson.Content = dto.Content;
        lesson.VideoUrl = dto.VideoUrl;
        lesson.Order = dto.Order;
        lesson.DurationMinutes = dto.DurationMinutes;

        await lessonRepository.UpdateAsync(lesson);
        return Ok(lesson);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid courseId, Guid id)
    {
        var course = await courseRepository.GetByIdAsync(courseId);
        if (course is null) return NotFound(new { error = "Course not found." });

        if (!IsAdmin && course.InstructorId != UserId) return Forbid();

        var lesson = await lessonRepository.GetByIdAsync(id);
        if (lesson is null || lesson.CourseId != courseId) return NotFound(new { error = "Lesson not found." });

        await lessonRepository.DeleteAsync(lesson);
        return NoContent();
    }
}
