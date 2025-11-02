using System;

namespace FitBridge_Application.Dtos.Payments.ApplePaymentDto;

public class AsnDecodedPayload
{
    public string NotificationType { get; init; } = default!;  
    public string? Subtype { get; init; }                   
    public AsnData Data { get; init; } = default!;
    public string? ExternalPurchaseToken { get; init; }
    public string Version { get; init; } = default!;          
    public long SignedDate { get; init; }     
    public Guid NotificationUUID { get; init; }
}
