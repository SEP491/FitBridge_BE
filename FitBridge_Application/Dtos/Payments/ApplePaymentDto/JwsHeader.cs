using System;

namespace FitBridge_Application.Dtos.Payments.ApplePaymentDto;

public class JwsHeader
{
    public string alg { get; init; } = default!;     
    public string kid { get; init; } = default!;
    public string[] x5c { get; init; } = default!;   
}
