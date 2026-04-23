using Application.Common;
using Application.DTOs;
using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    JwtService jwtService,
    IEmailService emailService,
    ICacheService cacheService,
    IConfiguration config,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<Result<string>> LoginAsync(LoginDto model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
            return Result<string>.Failure("Invalid email or password.", 401);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
            return Result<string>.Failure("Invalid email or password.", 401);

        var cacheKey = $"user_roles:{user.Id}";
        var cachedRoles = await cacheService.GetAsync<List<string>>(cacheKey);

        IList<string> roles;
        if (cachedRoles is not null)
        {
            roles = cachedRoles;
        }
        else
        {
            roles = await userManager.GetRolesAsync(user);
            await cacheService.SetAsync(cacheKey, roles.ToList(), TimeSpan.FromMinutes(10));
        }

        var token = jwtService.GenerateToken(user, roles);
        logger.LogInformation("User {Email} logged in", user.Email);
        return Result<string>.Success(token);
    }

    public async Task<Result<bool>> RegisterAsync(RegisterDto model)
    {
        var allowedRoles = new[] { UserRoles.Student, UserRoles.Instructor };
        if (!allowedRoles.Contains(model.Role))
            return Result<bool>.Failure("Role must be 'Student' or 'Instructor'.");

        var user = new ApplicationUser
        {
            FullName = model.FullName,
            UserName = model.Email,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<bool>.Failure(errors);
        }

        var roleResult = await userManager.AddToRoleAsync(user, model.Role);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
            return Result<bool>.Failure(errors);
        }

        await cacheService.RemoveAsync("dashboard:summary");
        logger.LogInformation("New user registered: {Email} as {Role}", model.Email, model.Role);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ChangePasswordAsync(string userId, ChangePasswordDto model)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result<bool>.NotFound("User not found.");

        var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<bool>.Failure(errors);
        }

        await cacheService.RemoveAsync($"user:{userId}");
        await cacheService.RemoveAsync($"user_roles:{userId}");
        return Result<bool>.Success(true);
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(string userId)
    {
        var cacheKey = $"user:{userId}";
        var cached = await cacheService.GetAsync<UserDto>(cacheKey);
        if (cached is not null)
            return Result<UserDto>.Success(cached);

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result<UserDto>.NotFound("User not found.");

        var roles = await userManager.GetRolesAsync(user);
        var dto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            AvatarUrl = user.AvatarUrl,
            Roles = roles
        };

        await cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));
        return Result<UserDto>.Success(dto);
    }

    public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        // Always return 200 to avoid revealing whether the email exists
        if (user is null)
            return Result<bool>.Success(true);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var frontendUrl = config["Frontend:BaseUrl"];
        var resetLink = $"{frontendUrl}/reset-password?token={encodedToken}&email={user.Email}";

        await emailService.SendAsync(new EmailMessageDto
        {
            To = user.Email!,
            Subject = "Password Reset — Online Courses",
            Body = $"""
                <h2>Password Reset</h2>
                <p>You requested a password reset. Click the button below:</p>
                <a href="{resetLink}" style="padding:10px 20px;background:#4F46E5;color:white;border-radius:6px;text-decoration:none;">
                    Reset Password
                </a>
                <p>The link is valid for <strong>15 minutes</strong>.</p>
                <p>If you did not request this, please ignore this email.</p>
            """
        });

        logger.LogInformation("Password reset email sent to {Email}", model.Email);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto model)
    {
        if (model.NewPassword != model.ConfirmPassword)
            return Result<bool>.Failure("Passwords do not match.");

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is null)
            return Result<bool>.Failure("User not found.");

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        var result = await userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<bool>.Failure(errors);
        }

        await cacheService.RemoveAsync($"user:{user.Id}");
        await cacheService.RemoveAsync($"user_roles:{user.Id}");
        return Result<bool>.Success(true);
    }
}
