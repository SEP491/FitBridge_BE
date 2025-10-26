using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.UpdatePassword;

public class UpdatePasswordCommandHandler(IApplicationUserService _applicationUserService, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork) : IRequestHandler<UpdatePasswordCommand, bool>
{
    public async Task<bool> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new BusinessException("User not found");
        }
        var user = await _applicationUserService.GetByIdAsync(userId.Value, isTracking: true);
        if (user == null)
        {
            throw new BusinessException("User not found");
        }
        var result = await _applicationUserService.UpdatePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        await _unitOfWork.CommitAsync();
        return result;
    }
}
