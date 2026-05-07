using System;
using Microsoft.AspNetCore.Identity;
using OnlineCourses.Domain.Entities;

namespace OnlineCourses.Domain.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<VerificationCode> VerificationCodes { get; set; } = new List<VerificationCode>();
}
