using System;

namespace FitBridge_Application.Dtos.Payments;

public class PaymentDataDto
{
    public string Bin { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public long OrderCode { get; set; }
    public string Currency { get; set; } = "VND";
    public string PaymentLinkId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
    public string QrCode { get; set; } = string.Empty;
}
