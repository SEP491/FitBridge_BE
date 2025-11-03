using System;
using FitBridge_Domain.Entities.Systems;
using FitBridge_Application.Dtos.SystemConfigs;
using MediatR;

namespace FitBridge_Application.Features.SystemConfigurations;

public class GetAllSystemConfigurationsQuery : IRequest<List<SystemConfigurationDto>>
{
}
