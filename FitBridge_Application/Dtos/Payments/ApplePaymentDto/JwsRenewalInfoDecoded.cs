using System;

namespace FitBridge_Application.Dtos.Payments.ApplePaymentDto;

public class JwsRenewalInfoDecoded
{
    public string OriginalTransactionId { get; init; } = default!;
    public string AutoRenewProductId { get; init; } = default!;
    public int AutoRenewStatus { get; init; }
    public string? OfferIdentifier { get; init; }
    public int? OfferType { get; init; }
    public string? PriceIncreaseStatus { get; init; }                
    public string Environment { get; init; } = default!;
    public long? RecentSubscriptionStartDate { get; init; }
    public long? RenewalDate { get; init; }
}
