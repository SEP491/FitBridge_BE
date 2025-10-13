using System;
using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using FitBridge_Application.Configurations;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FitBridge_Infrastructure.Services.Uploads;

public class UploadService(IOptions<AppWriteSettings> _appWriteSettings, Storage _storage) : IUploadService
{
    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file uploaded.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"};
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Only image files are allowed (.jpg, .jpeg, .png, .gif, .bmp, .webp).");

        if (string.IsNullOrEmpty(file.FileName))
            throw new ArgumentException("File name cannot be empty.");
        if (string.IsNullOrEmpty(file.ContentType))
            throw new ArgumentException("File content type cannot be empty.");

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var inputFile = InputFile.FromBytes(fileBytes, file.FileName, file.ContentType);

            var uploadFile = await _storage.CreateFile(
                bucketId: _appWriteSettings.Value.Bucket,
                fileId: Guid.NewGuid().ToString(),
                file: inputFile,
                permissions: new List<string> 
                {
                    Permission.Read(Role.Any())
                }
            );

            var fileUrl = $"{_appWriteSettings.Value.EndPoint}/storage/buckets/{_appWriteSettings.Value.Bucket}/files/{uploadFile.Id}/view?project={_appWriteSettings.Value.ProjectId}";
            return fileUrl;
        }
        catch (AppwriteException ex)
        {
            throw new BusinessException($"Upload failed: {ex.Message}");
        }
    }
}
