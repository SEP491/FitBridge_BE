using System;

namespace FitBridge_Application.Dtos.Payments.ApplePaymentDto;

public class JwsRenewalInfoDecoded
{
    public string OriginalTransactionId { get; init; } = default!;
    public string AutoRenewProductId { get; init; } = default!;
    public bool AutoRenewStatus { get; init; }
    public string? OfferIdentifier { get; init; }
    public string? OfferType { get; init; }
    public string? PriceIncreaseStatus { get; init; }                
    public string Environment { get; init; } = default!;
    public string? RecentSubscriptionStartDate { get; init; }
    public string? RenewalDate { get; init; }
}
