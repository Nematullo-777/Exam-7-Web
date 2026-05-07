using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Auth.Request;
using OnlineCourses.Application.DTOs.Auth.Response;

namespace OnlineCourses.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<Result<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto);
    Task<Result<bool>> ChangePasswordAsync(string id, ChangePasswordDto changePasswordDto);
    Task<Result<bool>> SendEmailAsync(SendEmailDto request);
    Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto request);
    Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request);
    Task<Result<bool>> AssisgnRoleAsync(string userId, string role);
}
