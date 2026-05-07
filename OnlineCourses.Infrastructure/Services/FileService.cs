using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<Result<string?>> UploadAsync(IFormFile file, string folderName)
    {
        try
        {
            if (file == null || file.Length == 0)
                return Result<string?>.Fail("File is empty", ErrorType.NotFound);

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/{folderName}/{fileName}";

            return Result<string?>.Ok(relativePath);
        }
        catch (System.Exception ex)
        {
            return Result<string?>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public Task<Result<bool>> DeleteAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.FromResult(Result<bool>.Fail("Invalid file path"));

        var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));

        if (!File.Exists(fullPath))
            return Task.FromResult(Result<bool>.Fail("File not found"));

        File.Delete(fullPath);

        return Task.FromResult(Result<bool>.Ok(true));
    }
}
