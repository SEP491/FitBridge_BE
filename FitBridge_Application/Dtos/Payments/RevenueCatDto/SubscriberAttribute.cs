using System;

namespace FitBridge_Application.Dtos.Payments.RevenueCatDto;

public class SubscriberAttribute
{
    public string Value { get; init; } = default!;
    public long UpdatedAtMs { get; init; }
}
