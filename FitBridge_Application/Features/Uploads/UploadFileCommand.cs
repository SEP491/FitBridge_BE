using System;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Uploads;

public class UploadFileCommand : IRequest<string>
{
    public IFormFile File { get; set; }
}
