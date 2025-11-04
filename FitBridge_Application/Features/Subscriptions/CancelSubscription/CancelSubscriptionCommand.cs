using System;
using MediatR;

namespace FitBridge_Application.Features.Subscriptions.CancelSubscription;

public class CancelSubscriptionCommand : IRequest<bool>
{
    public Guid userSubscriptionId { get; set; }
}
