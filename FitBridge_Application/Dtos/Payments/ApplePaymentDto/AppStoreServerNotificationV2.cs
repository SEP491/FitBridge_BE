using System;

namespace FitBridge_Application.Dtos.Payments.ApplePaymentDto;

public class AppStoreServerNotificationV2
{
    public string SignedPayload { get; init; } = default!;
}
