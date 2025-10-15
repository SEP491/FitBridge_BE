using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.ActivitySets.GetActivitySetsBySessionActivityId;

public class GetActivitySetsBySessionActivityIdSpecification : BaseSpecification<ActivitySet>
{
    public GetActivitySetsBySessionActivityIdSpecification(Guid sessionActivityId) : base(x => x.SessionActivityId == sessionActivityId)
    {
        AddInclude(x => x.SessionActivity);
    }
}
