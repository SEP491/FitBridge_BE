using System;
using MediatR;

namespace FitBridge_Application.Features.Payments.AppleWebhook;

public class AppleWebhookCommand : IRequest<bool>
{
    public string WebhookData { get; set; } = string.Empty;
}
