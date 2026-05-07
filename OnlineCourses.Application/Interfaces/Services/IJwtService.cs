using System;
using OnlineCourses.Domain.Identity;

namespace OnlineCourses.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(AppUser user, IList<string> roles);
}
