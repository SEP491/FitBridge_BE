using System;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.SystemConfigs;

namespace FitBridge_Domain.Entities.Systems;

public class SystemConfiguration : BaseEntity
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string? Description { get; set; }
    public SystemConfigurationDataType DataType { get; set; }
    public Guid UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
