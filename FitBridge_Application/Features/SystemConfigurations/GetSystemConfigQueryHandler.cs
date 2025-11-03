using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Systems;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Specifications.SystemConfigs;
using FitBridge_Application.Services;

namespace FitBridge_Application.Features.SystemConfigurations;

public class GetSystemConfigQueryHandler(SystemConfigurationService systemConfigurationService) : IRequestHandler<GetSystemConfigQuery, object>
{
    public async Task<object> Handle(GetSystemConfigQuery request, CancellationToken cancellationToken)
    {
        return await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(request.Key);
    }

}
