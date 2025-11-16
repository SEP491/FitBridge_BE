using FitBridge_Application.Dtos.Shippings;

namespace FitBridge_Application.Interfaces.Services;

public interface IAhamoveWebhookService
{
    Task ProcessWebhookAsync(AhamoveWebhookDto webhookData);
}

