using System;
using MediatR;
using System.Text.Json.Serialization;
using FitBridge_Domain.Enums.SystemConfigs;

namespace FitBridge_Application.Features.SystemConfigurations;

public class UpdateSystemConfigurationCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
}
