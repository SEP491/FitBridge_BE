using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Accounts;

namespace FitBridge_Application.Specifications.GoalTrainings;

public class GetAllGoalTrainingSpecification : BaseSpecification<GoalTraining>
{
    public GetAllGoalTrainingSpecification() : base(x => x.IsEnabled)
    {
        AddOrderBy(x => x.Name);
    }
}
