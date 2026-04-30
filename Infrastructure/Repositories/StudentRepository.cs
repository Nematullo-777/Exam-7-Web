using Application.Interfaces.Repositories;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StudentRepository(
    AppDbContext context,
    UserManager<ApplicationUser> userManager) : IStudentRepository
{
    public async Task<List<ApplicationUser>> GetAllStudentsAsync(int page, int pageSize)
    {
        var studentIds = await context.UserRoles
            .Join(context.Roles.Where(r => r.Name == UserRoles.Student),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => ur.UserId)
            .ToListAsync();

        return await context.Users
            .Where(u => studentIds.Contains(u.Id))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id) =>
        await userManager.FindByIdAsync(id);

    public async Task<int> GetTotalCountAsync()
    {
        var studentIds = await context.UserRoles
            .Join(context.Roles.Where(r => r.Name == UserRoles.Student),
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => ur.UserId)
            .ToListAsync();

        return studentIds.Count;
    }

    public async Task<ApplicationUser> UpdateAsync(ApplicationUser user)
    {
        await userManager.UpdateAsync(user);
        return user;
    }

    public async Task DeleteAsync(ApplicationUser user)
    {
        await userManager.DeleteAsync(user);
    }
}
