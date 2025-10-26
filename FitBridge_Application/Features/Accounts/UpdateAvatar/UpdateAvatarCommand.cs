using System;
using Microsoft.AspNetCore.Http;
using MediatR;

namespace FitBridge_Application.Features.Accounts.UpdateAvatar;

public class UpdateAvatarCommand : IRequest<string>
{
    public IFormFile Avatar { get; set; }
}
