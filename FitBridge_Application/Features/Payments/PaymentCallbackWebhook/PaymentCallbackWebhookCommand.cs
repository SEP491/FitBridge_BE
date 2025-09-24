using System;
using MediatR;
using Microsoft.VisualBasic;

namespace FitBridge_Application.Features.Payments.PaymentCallbackWebhook;

public class PaymentCallbackWebhookCommand : IRequest<bool>
{
    public string WebhookData { get; set; } = string.Empty;
}
