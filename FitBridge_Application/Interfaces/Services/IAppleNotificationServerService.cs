using System;

namespace FitBridge_Application.Interfaces.Services;

public interface IAppleNotificationServerService
{
    Task<bool> HandleAppleWebhookAsync(string webhookData);
}
