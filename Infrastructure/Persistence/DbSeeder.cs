using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class DbSeeder
{
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

        if (await userManager.FindByEmailAsync("admin@courses.com") is null)
        {
            var admin = new ApplicationUser
            {
                FullName = "Administrator",
                UserName = "admin@courses.com",
                Email = "admin@courses.com"
            };
            await userManager.CreateAsync(admin, "Admin@123!");
            await userManager.AddToRoleAsync(admin, UserRoles.Admin);
        }
    }
}
