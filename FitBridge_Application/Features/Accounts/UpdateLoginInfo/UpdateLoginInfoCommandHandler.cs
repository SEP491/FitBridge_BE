using System;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Exceptions;
using MediatR;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Interfaces.Repositories;
namespace FitBridge_Application.Features.Accounts.UpdateLoginInfo;

public class UpdateLoginInfoCommandHandler(IApplicationUserService _applicationUserService, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork) : IRequestHandler<UpdateLoginInfoCommand, bool>
{
    public async Task<bool> Handle(UpdateLoginInfoCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new BusinessException("User not found");
        }
        var user = await _applicationUserService.GetByIdAsync(userId.Value, isTracking: true);
        if (request.Email == null && request.PhoneNumber == null)
        {
            throw new NotFoundException("User not found");
        }
        if (user == null)
        {
            throw new BusinessException("User not found");
        }
        await _applicationUserService.UpdateLoginInfoAsync(user, request.Email, request.PhoneNumber);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
