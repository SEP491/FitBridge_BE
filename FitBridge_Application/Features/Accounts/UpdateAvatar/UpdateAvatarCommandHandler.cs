using System;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.UpdateAvatar;

public class UpdateAvatarCommandHandler(IUploadService _uploadService, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IApplicationUserService _applicationUserService, IUnitOfWork _unitOfWork) : IRequestHandler<UpdateAvatarCommand, string>
{
    public async Task<string> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        if(request.Avatar.Length > ProjectConstant.MaximumAvatarSize * 1024 * 1024)
        {
            throw new BusinessException($"Avatar size is too large, maximum size is {ProjectConstant.MaximumAvatarSize}MB");
        }
        var avatarUrl = await _uploadService.UploadFileAsync(request.Avatar);
        var account = await _applicationUserService.GetByIdAsync(userId.Value, isTracking: true);
        if (account == null)
        {
            throw new NotFoundException("Account not found");
        }
        account.AvatarUrl = avatarUrl;
        await _unitOfWork.CommitAsync();
        return avatarUrl;
    }
}
