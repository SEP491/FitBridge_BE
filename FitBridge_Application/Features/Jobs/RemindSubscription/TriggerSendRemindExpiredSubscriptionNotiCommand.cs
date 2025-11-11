using System;
using MediatR;

namespace FitBridge_Application.Features.Jobs.RemindSubscription;

public class TriggerSendRemindExpiredSubscriptionNotiCommand : IRequest<bool>
{
    public Guid UserSubscriptionId { get; set; }
}
