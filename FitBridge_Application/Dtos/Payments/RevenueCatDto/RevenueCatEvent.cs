using System;

namespace FitBridge_Application.Dtos.Payments.RevenueCatDto;

public class RevenueCatEvent
{
    public long EventTimestampMs { get; init; }
    public string ProductId { get; init; } = default!;
    public string PeriodType { get; init; } = default!;
    public long PurchasedAtMs { get; init; }
    public long ExpirationAtMs { get; init; }
    public string Environment { get; init; } = default!;
    public string? EntitlementId { get; init; }
    public List<string>? EntitlementIds { get; init; }
    public string? PresentedOfferingId { get; init; }
    public string? TransactionId { get; init; }
    public string? OriginalTransactionId { get; init; }
    public bool? IsFamilyShare { get; init; }
    public string? CountryCode { get; init; }
    public string AppUserId { get; init; } = default!;
    public List<string> Aliases { get; init; } = new();
    public string OriginalAppUserId { get; init; } = default!;
    public string? Currency { get; init; }
    public decimal? Price { get; init; }
    public decimal? PriceInPurchasedCurrency { get; init; }
    public Dictionary<string, SubscriberAttribute> SubscriberAttributes { get; init; } = new();
    public string Store { get; init; } = default!;
    public decimal? TakehomePercentage { get; init; }
    public string? OfferCode { get; init; }
    public decimal? TaxPercentage { get; init; }
    public decimal? CommissionPercentage { get; init; }
    public string? Metadata { get; init; }
    public int? RenewalNumber { get; init; }
    public string Type { get; init; } = default!;
    public string Id { get; init; } = default!;
    public string AppId { get; init; } = default!;
}
