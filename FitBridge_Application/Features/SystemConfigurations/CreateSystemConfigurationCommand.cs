using System;
using MediatR;
using FitBridge_Domain.Enums.SystemConfigs;

namespace FitBridge_Application.Features.SystemConfigurations;

public class CreateSystemConfigurationCommand : IRequest<string>
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string? Description { get; set; }
    public SystemConfigurationDataType DataType { get; set; }
}
