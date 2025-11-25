using System;
using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using FitBridge_Application.Commons.Constants;
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
        {
            throw new BusinessException("No file uploaded.");
        }
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (fileExtension == ".pdf")
        {
            if (file.Length > ProjectConstant.MaximumContractSize * 1024 * 1024)
            {
                throw new BusinessException($"File size is too large, maximum size is {ProjectConstant.MaximumContractSize}MB");
            }
        }
        else
        {
            if (file.Length > ProjectConstant.MaximumAvatarSize * 1024 * 1024)
            {
                throw new BusinessException($"File size is too large, maximum size is {ProjectConstant.MaximumAvatarSize}MB");
            }
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".pdf" };
        if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            throw new ArgumentException("Only image and pdf files are allowed (.jpg, .jpeg, .png, .gif, .bmp, .webp, .pdf).");

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

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            var fileId = ExtractFileIdFromUrl(fileUrl);
            if (string.IsNullOrEmpty(fileId))
            {
                throw new BusinessException("Invalid file URL");
            }

            await _storage.DeleteFile(
                bucketId: _appWriteSettings.Value.Bucket,
                fileId: fileId
            );

            return true;
        }
        catch (AppwriteException ex)
        {
            throw new BusinessException($"Delete failed: {ex.Message}");
        }
    }

    private string ExtractFileIdFromUrl(string fileUrl)
    {
        var parts = fileUrl.Split('/');
        var filesIndex = Array.IndexOf(parts, "files");

        if (filesIndex >= 0 && filesIndex + 1 < parts.Length)
        {
            var fileIdPart = parts[filesIndex + 1];
            return fileIdPart.Split('?')[0]; // Remove query parameters
        }

        return string.Empty;
    }
}
