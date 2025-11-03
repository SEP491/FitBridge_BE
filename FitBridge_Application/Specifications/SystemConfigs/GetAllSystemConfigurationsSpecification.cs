using System;
using FitBridge_Domain.Entities.Systems;

namespace FitBridge_Application.Specifications.SystemConfigs;

public class GetAllSystemConfigurationsSpecification : BaseSpecification<SystemConfiguration>
{
    public GetAllSystemConfigurationsSpecification() : base(x => x.IsEnabled)
    {
    }
}
