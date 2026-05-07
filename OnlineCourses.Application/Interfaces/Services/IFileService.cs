using System;
using Microsoft.AspNetCore.Http;
using OnlineCourses.Application.Common;

namespace OnlineCourses.Application.Interfaces.Services;

public interface IFileService
{
    Task<Result<string?>> UploadAsync(IFormFile file, string folderName);
    Task<Result<bool>> DeleteAsync(string filePath);
}
