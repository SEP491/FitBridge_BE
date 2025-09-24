using System;
using FitBridge_Application.Dtos.Payments;
using MediatR;

namespace FitBridge_Application.Features.Payments.GetPaymentInfor;

public class GetPaymentInfoCommand : IRequest<PaymentInfoResponseDto>
{
    public string Id { get; set; } = string.Empty;
}
