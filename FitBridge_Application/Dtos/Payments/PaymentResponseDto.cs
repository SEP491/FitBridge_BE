using System;

namespace FitBridge_Application.Dtos.Payments;

public class PaymentResponseDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PaymentDataDto? Data { get; set; }
    public string Signature { get; set; } = string.Empty;
}
