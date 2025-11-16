using MediatR;

namespace FitBridge_Application.Features.Orders.ProcessAhamoveWebhook;

public class ProcessAhamoveWebhookCommand : IRequest<bool>
{
    public string WebhookPayload { get; set; }
}

