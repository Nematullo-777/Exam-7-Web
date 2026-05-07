using System;
using OnlineCourses.Domain.Entities;

namespace OnlineCourses.Application.Interfaces.Repositories;

public interface IVerificationCodeRepository
{
    Task AddAsync(VerificationCode code);
    Task<VerificationCode?> GetLatestVerificationCodeAsync(string userId);
    Task<VerificationCode?> GetVerificationCodeAsync(int id);
}
