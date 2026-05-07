using System;
using Microsoft.AspNetCore.Identity;
using OnlineCourses.Domain.Constants;
using OnlineCourses.Domain.Identity;

namespace OnlineCourses.Infrastructure.Seeds;

public static class UserSeeder
{
    public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
    {
        var email = "admin@gmail.com";
        var password = "Admin@123";

        var admin = await userManager.FindByEmailAsync(email);

        if (admin == null)
        {
            var user = new AppUser
            {
                FullName = "Admin Adminov",
                Email = email,
                UserName = "admin",
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
        }
    }
}