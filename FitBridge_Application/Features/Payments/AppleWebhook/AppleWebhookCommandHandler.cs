using System;
using MediatR;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Payments.AppleWebhook;

public class AppleWebhookCommandHandler(IAppleNotificationServerService _appleNotificationServerService) : IRequestHandler<AppleWebhookCommand, bool>
{
    public async Task<bool> Handle(AppleWebhookCommand request, CancellationToken cancellationToken)
    {
        var result = await _appleNotificationServerService.HandleAppleWebhookAsync(request.WebhookData);
        return result;
    }
}
