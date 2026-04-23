using Application.DTOs.AuthDTOs;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = UserRoles.Admin)]
public class UsersController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 50) pageSize = 50;

        var totalCount = await userManager.Users.CountAsync();
        var users = await userManager.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            dtos.Add(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                AvatarUrl = user.AvatarUrl,
                Roles = roles
            });
        }

        return Ok(new { items = dtos, totalCount, page, pageSize });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound(new { error = "User not found." });

        var roles = await userManager.GetRolesAsync(user);
        return Ok(new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            AvatarUrl = user.AvatarUrl,
            Roles = roles
        });
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleDto dto)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound(new { error = "User not found." });

        if (!await roleManager.RoleExistsAsync(dto.Role))
            return BadRequest(new { error = $"Role '{dto.Role}' does not exist." });

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRoleAsync(user, dto.Role);

        return Ok(new { message = $"User role updated to '{dto.Role}'." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound(new { error = "User not found." });

        await userManager.DeleteAsync(user);
        return NoContent();
    }
}

public class UpdateRoleDto
{
    public string Role { get; set; } = string.Empty;
}
