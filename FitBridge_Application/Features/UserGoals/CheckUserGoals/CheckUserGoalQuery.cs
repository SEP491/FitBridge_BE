using System;
using MediatR;

namespace FitBridge_Application.Features.UserGoals.CheckUserGoals;

public class CheckUserGoalQuery : IRequest<bool>
{
    public Guid CustomerPurchasedId { get; set; }
}
