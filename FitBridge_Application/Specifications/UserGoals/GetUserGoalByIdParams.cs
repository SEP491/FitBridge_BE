using System;
namespace FitBridge_Application.Specifications.UserGoals;

public class GetUserGoalByIdParams : BaseParams
{
    public Guid Id { get; set; }
}
