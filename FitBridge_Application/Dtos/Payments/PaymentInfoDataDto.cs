using System;

namespace FitBridge_Application.Dtos.Payments;

public class PaymentInfoDataDto
{
    public string Id { get; set; } = string.Empty;
    public int OrderCode { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountRemaining { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CanceledAt { get; set; }
}