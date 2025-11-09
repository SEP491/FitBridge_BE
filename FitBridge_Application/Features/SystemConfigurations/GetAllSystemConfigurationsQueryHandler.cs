using System;
using FitBridge_Application.Dtos.SystemConfigs;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Systems;
using MediatR;
using AutoMapper;
using FitBridge_Application.Specifications.SystemConfigs;

namespace FitBridge_Application.Features.SystemConfigurations;

public class GetAllSystemConfigurationsQueryHandler(IUnitOfWork _unitOfWork, IConfigurationProvider mapperConfig) : IRequestHandler<GetAllSystemConfigurationsQuery, List<SystemConfigurationDto>>
{
    public async Task<List<SystemConfigurationDto>> Handle(GetAllSystemConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var systemConfigurations = await _unitOfWork.Repository<SystemConfiguration>().GetAllWithSpecificationProjectedAsync<SystemConfigurationDto>(new GetAllSystemConfigurationsSpecification(), mapperConfig);
        return systemConfigurations.ToList();
    }
}
