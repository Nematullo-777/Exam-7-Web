using System;
using OnlineCourses.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using OnlineCourses.Application.DTOs.Auth.Request;
using OnlineCourses.Application.DTOs.Auth.Response;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Identity;
using OnlineCourses.Application.Common;
using OnlineCourses.Domain.Constants;
using OnlineCourses.Application.Interfaces.Repositories;
using System.Net.Mail;
using OnlineCourses.Domain.Entities;

namespace OnlineCourses.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly ICacheService _cacheService;
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IJwtService jwtService, IEmailService emailService, ICacheService cacheService, IVerificationCodeRepository verificationCodeRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _emailService = emailService;
        _cacheService = cacheService;
        _verificationCodeRepository = verificationCodeRepository;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return Result<LoginResponseDto>.Fail($"User with Email = {loginDto.Email} not found", ErrorType.NotFound);

            var checkPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!checkPassword)
                return Result<LoginResponseDto>.Fail("Invalid password or email", ErrorType.Validation);

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtService.GenerateToken(user, roles);

            return Result<LoginResponseDto>.Ok(new LoginResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                UserName = user.UserName!,
                Role = roles.FirstOrDefault() ?? "Student",
                Token = token
            });
        }
        catch (System.Exception ex)
        {
            return Result<LoginResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto.Password.Length < 6)
            return Result<RegisterResponseDto>.Fail("Password cannot be less than 6", ErrorType.Validation);

        if (registerDto.Password != registerDto.ConfirmPassword)
            return Result<RegisterResponseDto>.Fail("Password do not match", ErrorType.Validation);
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
                return Result<RegisterResponseDto>.Fail("User already exists", ErrorType.Conflict);

            var user = new AppUser
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.UserName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                return Result<RegisterResponseDto>.Fail(errors, ErrorType.Validation);
            }

            var allowedRoles = new[] { UserRoles.Student, UserRoles.Instructor };
            var roleToAssign = allowedRoles.Contains(registerDto.Role) ? registerDto.Role : UserRoles.Student;
            await _userManager.AddToRoleAsync(user, roleToAssign);

            return Result<RegisterResponseDto>.Ok(new RegisterResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                Role = roleToAssign

            });
        }
        catch (System.Exception ex)
        {
            return Result<RegisterResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> ChangePasswordAsync(string id, ChangePasswordDto changePasswordDto)
    {
        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            return Result<bool>.Fail("Password do not match", ErrorType.Conflict);
        try
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return Result<bool>.Fail("User not found", ErrorType.NotFound);

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                return Result<bool>.Fail(errors, ErrorType.Validation);
            }

            return Result<bool>.Ok(true);
        }
        catch (System.Exception ex)
        {
            return Result<bool>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<bool>> SendEmailAsync(SendEmailDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        var code = new Random().Next(100_000, 999_999).ToString();
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<style>
    body {{
        margin: 0;
        padding: 0;
        background: #f2f4f8;
        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Arial, sans-serif;
    }}

    .container {{
        max-width: 480px;
        margin: 40px auto;
        background: #ffffff;
        border-radius: 16px;
        overflow: hidden;
        box-shadow: 0 10px 30px rgba(0,0,0,0.08);
    }}

    .header {{
        background: linear-gradient(135deg, #4f46e5, #7c3aed);
        padding: 30px;
        text-align: center;
    }}

    .header h1 {{
        color: white;
        margin: 0;
        font-size: 22px;
        letter-spacing: 0.5px;
    }}

    .body {{
        padding: 30px;
        text-align: center;
    }}

    .body p {{
        color: #444;
        font-size: 15px;
        line-height: 1.6;
        margin: 10px 0;
    }}

    .code-box {{
        margin: 25px 0;
    }}

    .code {{
        display: inline-block;
        font-size: 34px;
        font-weight: bold;
        color: #4f46e5;
        background: #f5f7ff;
        padding: 14px 30px;
        border-radius: 10px;
        letter-spacing: 10px;
        border: 1px solid #e0e7ff;
    }}

    .button {{
        display: inline-block;
        margin-top: 20px;
        padding: 12px 25px;
        background: #4f46e5;
        color: white;
        text-decoration: none;
        border-radius: 8px;
        font-size: 14px;
    }}

    .warning {{
        color: #888;
        font-size: 13px;
        margin-top: 15px;
    }}

    .footer {{
        background: #fafafa;
        padding: 15px;
        text-align: center;
        color: #aaa;
        font-size: 12px;
        border-top: 1px solid #eee;
    }}

    @media (max-width: 500px) {{
        .container {{
            margin: 20px;
        }}
    }}
</style>
</head>

<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Password Reset</h1>
        </div>

        <div class='body'>
            <p>Hello, <strong>{user.UserName}</strong></p>
            <p>We received a request to reset your password.</p>

            <div class='code-box'>
                <div class='code'>{code}</div>
            </div>

            <p>If the code doesn’t work, copy it manually.</p>

            <p class='warning'>⏱ This code will expire in <strong>3 minutes</strong>.</p>
            <p class='warning'>If you didn’t request this, you can safely ignore this email.</p>
        </div>

        <div class='footer'>
            © 2026 OnlineCourses — Secure Authentication System
        </div>
    </div>
</body>
</html>";
        try
        {
            var result = await _emailService.SendEmailAsync(request.Email, "Password reset", htmlBody);
            if (!result)
            {
                return Result<bool>.Fail("Failed to send email");
            }
        }
        catch (SmtpException e)
        {
            return Result<bool>.Fail("Failed to send email: " + e.Message);
        }

        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = code,
            Expiration = DateTime.UtcNow.AddMinutes(3)
        };

        await _verificationCodeRepository.AddAsync(verificationCode);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> VerifyCodeAsync(VerifyCodeDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        var lastCode = await _verificationCodeRepository
            .GetLatestVerificationCodeAsync(user.Id);

        if (lastCode == null)
            return Result<bool>.Fail("Code not found", ErrorType.NotFound);

        if (lastCode.Code != request.Code)
            return Result<bool>.Fail("Invalid code", ErrorType.Validation);

        if (lastCode.Expiration < DateTime.UtcNow)
            return Result<bool>.Fail("Code expired", ErrorType.Validation);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        if (request.NewPassword != request.ConfirmPassword)
            return Result<bool>.Fail("Passwords do not match", ErrorType.Validation);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            request.NewPassword
        );

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            return Result<bool>.Fail(errors, ErrorType.Validation);
        }

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> AssisgnRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<bool>.Fail("User not found", ErrorType.NotFound);

        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
            return Result<bool>.Fail("Role does not exist", ErrorType.Validation);

        var alreadyInRole = await _userManager.IsInRoleAsync(user, role);
        if (alreadyInRole)
            return Result<bool>.Fail($"User already has role '{role}'", ErrorType.Validation);

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<bool>.Fail(errors, ErrorType.Validation);
        }

        await _cacheService.RemoveAsync("all_users");
        await _cacheService.RemoveAsync($"user_{userId}");

        return Result<bool>.Ok(true);
    }
}
