using System;
using Microsoft.AspNetCore.Http;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Students.Request;
using OnlineCourses.Application.DTOs.Students.Response;

namespace OnlineCourses.Application.Interfaces.Services;

public interface IStudentService
{
    Task<Result<List<GetStudentDto>>> GetAllStudentsAsync();
    Task<Result<GetStudentDto?>> GetStudentByIdAsync(string id);
    Task<Result<UpdateStudentResponseDto>> UpdateStudentAsync(string id, UpdateStudentDto updateStudentDto);
    Task<Result<bool>> UpdateStudentProfileAsync(string userId, IFormFile file);
    Task<Result<DeleteStudentResponseDto>> DeleteStudentAsync(string id);
    Task<Result<bool>> DeleteStudentProfileAsync(string userId);
}
