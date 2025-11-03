using System;
using FitBridge_Domain.Entities.Systems;

namespace FitBridge_Application.Specifications.SystemConfigs;

public class GetSystemConfigurationByKeySpecification : BaseSpecification<SystemConfiguration>  
{
    public GetSystemConfigurationByKeySpecification(string key)
        : base(e => e.IsEnabled && e.Key == key)
    {
    }
}
