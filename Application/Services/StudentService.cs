using Application.Common;
using Application.DTOs.StudentDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class StudentService(IStudentRepository studentRepository) : IStudentService
{
    public async Task<Result<PagedResult<StudentDto>>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var users = await studentRepository.GetAllStudentsAsync(page, pageSize);
        var totalCount = await studentRepository.GetTotalCountAsync();

        var dtos = users.Select(u => new StudentDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email!,
            AvatarUrl = u.AvatarUrl,
        }).ToList();

        return Result<PagedResult<StudentDto>>.Success(new PagedResult<StudentDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<Result<StudentDto>> GetByIdAsync(string id)
    {
        var user = await studentRepository.GetByIdAsync(id);
        if (user is null)
            return Result<StudentDto>.NotFound("Student not found.");

        return Result<StudentDto>.Success(new StudentDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            // Bio = user.Student?.Bio,
            AvatarUrl = user.AvatarUrl,
            // EnrollmentCount = user.Student?.Enrollments?.Count ?? 0
        });
    }

    public async Task<Result<StudentDto>> UpdateAsync(string id, string requestUserId, bool isAdmin, UpdateStudentDto dto)
    {
        if (!isAdmin && id != requestUserId)
            return Result<StudentDto>.Forbidden("You can only update your own profile.");

        var user = await studentRepository.GetByIdAsync(id);
        if (user is null)
            return Result<StudentDto>.NotFound("Student not found.");

        user.FullName = dto.FullName;
        user.AvatarUrl = dto.AvatarUrl;

        if (user.Student is not null)
        {
            user.Student.FullName = dto.FullName;
            user.Student.Bio = dto.Bio;
            user.Student.AvatarUrl = dto.AvatarUrl;
        }

        var updated = await studentRepository.UpdateAsync(user);

        return Result<StudentDto>.Success(new StudentDto
        {
            Id = updated.Id,
            FullName = updated.FullName,
            Email = updated.Email!,
            Bio = updated.Student?.Bio,
            AvatarUrl = updated.AvatarUrl,
            EnrollmentCount = updated.Student?.Enrollments?.Count ?? 0
        });
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        var user = await studentRepository.GetByIdAsync(id);
        if (user is null)
            return Result<bool>.NotFound("Student not found.");

        await studentRepository.DeleteAsync(user);
        return Result<bool>.Success(true);
    }
}
