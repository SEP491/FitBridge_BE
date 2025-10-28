using System;
using FitBridge_Application.Dtos.UserGoals;
using FitBridge_Application.Specifications.UserGoals;
using MediatR;

namespace FitBridge_Application.Features.UserGoals.GetUserGoalById;

public class GetUserGoalByIdQuery : IRequest<UserGoalsDto>
{
    public Guid CustomerPurchasedId { get; set; }
}
