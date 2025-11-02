using System;

namespace FitBridge_Application.Dtos.Payments.ApplePaymentDto;

public sealed class AsnData
{
    public long? AppAppleId { get; init; }
    public string BundleId { get; init; } = default!;
    public string? BundleVersion { get; init; }
    public string Environment { get; init; } = default!;          
    public string? ConsumptionRequestReason { get; init; }         
    public string? SignedRenewalInfo { get; init; }                
    public string SignedTransactionInfo { get; init; } = default!; 
    public int? Status { get; init; }                              
}