using System;
using FitBridge_Application.Interfaces.Services;
using MediatR;

namespace FitBridge_Application.Features.Uploads;

public class UploadFileCommandHandler(IUploadService _uploadService) : IRequestHandler<UploadFileCommand, string>
{
    public async Task<string> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        return await _uploadService.UploadFileAsync(request.File);
    }
}
