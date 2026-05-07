using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Students.Request;
using OnlineCourses.Application.DTOs.Students.Response;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Identity;
using OnlineCourses.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace OnlineCourses.Application.Services;

public class StudentService : IStudentService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ICacheService _cacheService;
    private readonly IFileService _fileService;

    public StudentService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ICacheService cacheService, IFileService fileService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _cacheService = cacheService;
        _fileService = fileService;
    }

    public async Task<Result<List<GetStudentDto>>> GetAllStudentsAsync()
    {
        var cahced = await _cacheService.GetAsync<List<GetStudentDto>>("all_students");
        if (cahced != null)
            return Result<List<GetStudentDto>>.Ok(cahced);

        var stduents = await _userManager.Users.ToListAsync();

        var result = new List<GetStudentDto>();

        foreach (var student in stduents)
        {
            var roles = await _userManager.GetRolesAsync(student);

            result.Add(new GetStudentDto
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email!,
                Username = student.UserName!,
                Role = roles.FirstOrDefault() ?? "Student",
                ImageUrl = student.ImageUrl
            });
        }

        await _cacheService.SetAsync("all_students", result, TimeSpan.FromMinutes(10));
        return Result<List<GetStudentDto>>.Ok(result);
    }

    public async Task<Result<GetStudentDto?>> GetStudentByIdAsync(string id)
    {
        var cached = await _cacheService.GetAsync<GetStudentDto>($"student_{id}");
        if (cached != null)
            return Result<GetStudentDto?>.Ok(cached);

        var student = await _userManager.FindByIdAsync(id);

        if (student == null)
            return Result<GetStudentDto?>.Fail("Student not found", ErrorType.NotFound);

        var roles = await _userManager.GetRolesAsync(student);

        var dto = new GetStudentDto
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email!,
            Username = student.UserName!,
            Role = roles.FirstOrDefault() ?? "Student",
            ImageUrl = student.ImageUrl
        };

        await _cacheService.SetAsync($"user_{id}", dto, TimeSpan.FromMinutes(10));

        return Result<GetStudentDto?>.Ok(dto);
    }

    public async Task<Result<UpdateStudentResponseDto>> UpdateStudentAsync(string id, UpdateStudentDto updateStudentDto)
    {
        if (string.IsNullOrWhiteSpace(updateStudentDto.FullName))
            return Result<UpdateStudentResponseDto>.Fail("FullName cannot be empty", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(updateStudentDto.Email))
            return Result<UpdateStudentResponseDto>.Fail("Email cannot be empty", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(updateStudentDto.Username))
            return Result<UpdateStudentResponseDto>.Fail("UserName cannot be empty", ErrorType.Validation);
        try
        {
            var student = await _userManager.FindByIdAsync(id);

            if (student == null)
                return Result<UpdateStudentResponseDto>.Fail("User not found for update", ErrorType.NotFound);

            student.FullName = updateStudentDto.FullName;
            student.Email = updateStudentDto.Email;
            student.UserName = updateStudentDto.Username;

            var updateResult = await _userManager.UpdateAsync(student);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return Result<UpdateStudentResponseDto>.Fail(errors, ErrorType.Validation);
            }

            var roles = await _userManager.GetRolesAsync(student);

            await _cacheService.RemoveAsync("all_students");
            await _cacheService.RemoveAsync($"student_{id}");

            return Result<UpdateStudentResponseDto>.Ok(new UpdateStudentResponseDto
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email!,
                Username = student.UserName!,
                Role = roles.FirstOrDefault() ?? "Student",
                ImageUrl = student.ImageUrl
            });
        }
        catch (System.Exception ex)
        {
            return Result<UpdateStudentResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<DeleteStudentResponseDto>> DeleteStudentAsync(string id)
    {
        try
        {
            var student = await _userManager.FindByIdAsync(id);

            if (student == null)
                return Result<DeleteStudentResponseDto>.Fail("User not found for delete", ErrorType.NotFound);

            var roles = await _userManager.GetRolesAsync(student);

            var deleteResult = await _userManager.DeleteAsync(student);
            if (!deleteResult.Succeeded)
            {
                var errors = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
                return Result<DeleteStudentResponseDto>.Fail(errors, ErrorType.Unknown);
            }

            await _cacheService.RemoveAsync("all_students");
            await _cacheService.RemoveAsync($"student_{id}");

            return Result<DeleteStudentResponseDto>.Ok(new DeleteStudentResponseDto
            {
                Id = student.Id,
                Username = student.UserName!,
                Email = student.Email!,
                Role = roles.FirstOrDefault() ?? "Student"
            });
        }
        catch (System.Exception ex)
        {
            return Result<DeleteStudentResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> UpdateStudentProfileAsync(string userId, IFormFile file)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<bool>.Fail("User not found", ErrorType.NotFound);

            if (!string.IsNullOrEmpty(user.ImageUrl))
                await _fileService.DeleteAsync(user.ImageUrl);

            var fileName = await _fileService.UploadAsync(file, "profile");
            if (fileName == null)
                return Result<bool>.Fail("File upload not found", ErrorType.NotFound);

            user.ImageUrl = fileName.Data;

            await _userManager.UpdateAsync(user);

            await _cacheService.RemoveAsync("all_users");
            await _cacheService.RemoveAsync($"user_{userId}");

            return Result<bool>.Ok(true);
        }
        catch (System.Exception ex)
        {
            return Result<bool>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> DeleteStudentProfileAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.Fail("User not found", ErrorType.NotFound);

            if (string.IsNullOrEmpty(user.ImageUrl))
                return Result<bool>.Fail("User has no profile image", ErrorType.NotFound);

            var deleted = await _fileService.DeleteAsync(user.ImageUrl);

            if (!deleted.Data)
                return Result<bool>.Fail("Failed to image delete", ErrorType.Unknown);

            user.ImageUrl = null;

            await _userManager.UpdateAsync(user);

            await _cacheService.RemoveAsync($"user_{userId}");
            await _cacheService.RemoveAsync("all_users");

            return Result<bool>.Ok(true);
        }
        catch (System.Exception ex)
        {
            return Result<bool>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }
}