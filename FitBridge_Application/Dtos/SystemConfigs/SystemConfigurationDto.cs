using System;
using FitBridge_Domain.Enums.SystemConfigs;

namespace FitBridge_Application.Dtos.SystemConfigs;

public class SystemConfigurationDto
{
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public string? Description { get; set; }
    public SystemConfigurationDataType DataType { get; set; }
}
