using System;

namespace FitBridge_Application.Dtos.Payments;

public class PaymentInfoResponseDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PaymentInfoDataDto? Data { get; set; }
    public string Signature { get; set; } = string.Empty;   
}
