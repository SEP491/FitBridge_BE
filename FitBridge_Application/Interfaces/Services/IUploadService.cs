using System;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Interfaces.Services;

public interface IUploadService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<bool> DeleteFileAsync(string fileUrl);
}
