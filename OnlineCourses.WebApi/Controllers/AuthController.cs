using System;
using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Application.DTOs.Auth.Request;
using OnlineCourses.Application.Interfaces.Services;

namespace OnlineCourses.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class AuthController : BaseController
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginDto loginDto)
    {
        var result = await _service.LoginAsync(loginDto);

        return !result.IsSuccess ? HandleError(result) : Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
    {
        var register = await _service.RegisterAsync(registerDto);

        return !register.IsSuccess ? HandleError(register) : Created("", register);
    }
    
    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(userId == null)
            return Unauthorized();

        var register = await _service.ChangePasswordAsync(userId, changePasswordDto);

        return !register.IsSuccess ? HandleError(register) : Ok(register);
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmailAsync(SendEmailDto sendEmailDto)
    {
        var res = await _service.SendEmailAsync(sendEmailDto);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCodeAsync(VerifyCodeDto verifyCodeDto)
    {
        var res = await _service.VerifyCodeAsync(verifyCodeDto);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var res = await _service.ResetPasswordAsync(resetPasswordDto);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }

    [HttpPost("assign-role/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRoleAsync(string id, string role)
    {
        var res = await _service.AssisgnRoleAsync(id, role);

        return !res.IsSuccess ? HandleError(res) : Ok(res);
    }
}
