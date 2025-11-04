using System;
using MediatR;

namespace FitBridge_Application.Features.Jobs.ExpiredSubscription;

public class TriggerExpireUserSubscriptionCommand : IRequest<bool>
{
    public Guid UserSubscriptionId { get; set; }
}
