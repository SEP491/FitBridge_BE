using System;

namespace FitBridge_Application.Dtos.Payments.ApplePaymentDto;

public class JwsTransactionDecoded
{
    public string TransactionId { get; init; } = default!;
    public string OriginalTransactionId { get; init; } = default!;
    public string BundleId { get; init; } = default!;
    public string ProductId { get; init; } = default!;
    public string? SubscriptionGroupIdentifier { get; init; }
    public string InAppOwnershipType { get; init; } = default!;     
    public string Type { get; init; } = default!;                    
    public long PurchaseDate { get; init; }                          
    public long? ExpiresDate { get; init; }                          
    public bool? IsUpgraded { get; init; }
    public string? OfferIdentifier { get; init; }
    public string? OfferType { get; init; }                          
    public string? Currency { get; init; }
    public long? Price { get; init; }                                
    public string Environment { get; init; } = default!;
    public string? CountryCode { get; init; }
}
