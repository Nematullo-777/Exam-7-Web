using System;
using Microsoft.AspNetCore.Identity;
using OnlineCourses.Domain.Constants;

namespace OnlineCourses.Infrastructure.Seeds;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            UserRoles.Admin,
            UserRoles.Instructor,
            UserRoles.Student
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}