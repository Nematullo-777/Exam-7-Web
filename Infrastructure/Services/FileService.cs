using Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class FileService(IWebHostEnvironment _env, ILogger<FileService> _logger) : IFileService
{
    public async Task<string?> UploadAsync(IFormFile file, string folderName)
    {
        try
        {
            if (file is null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var publicPath = Path.Combine("uploads", folderName, fileName).Replace("\\", "/");
            _logger.LogInformation("File uploaded: {Path}", publicPath);
            return publicPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File upload failed for folder {Folder}", folderName);
            return null;
        }
    }

    public Task<bool> DeleteAsync(string filePath)
    {
        var fullPath = Path.Combine(_env.WebRootPath, filePath);
        if (!File.Exists(fullPath))
            return Task.FromResult(false);

        File.Delete(fullPath);
        _logger.LogInformation("File deleted: {Path}", filePath);
        return Task.FromResult(true);
    }
}
