using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Systems;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.SystemConfigurations;

public class CreateSystemConfigurationCommandHandler(IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<CreateSystemConfigurationCommand, string>    
{
    public async Task<string> Handle(CreateSystemConfigurationCommand request, CancellationToken cancellationToken)
    {
        var accountId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (accountId == null)
        {
            throw new NotFoundException("Unauthorized");
        }
        var systemConfiguration = new SystemConfiguration
        {
            Key = request.Key,
            Value = request.Value,
            Description = request.Description,
            DataType = request.DataType,
            UpdatedById = accountId.Value,
        };
         _unitOfWork.Repository<SystemConfiguration>().Insert(systemConfiguration);
        await _unitOfWork.CommitAsync();
        return systemConfiguration.Key;
    }
}
