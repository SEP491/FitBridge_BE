using System;
using FitBridge_Application.Dtos.Payments;
using MediatR;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Payments.GetPaymentInfor;

public class GetPaymentInfoCommandHandler(IPayOSService _payOSService) : IRequestHandler<GetPaymentInfoCommand, PaymentInfoResponseDto>
{
    
    public async Task<PaymentInfoResponseDto> Handle(GetPaymentInfoCommand request, CancellationToken cancellationToken)
    {
        return await _payOSService.GetPaymentInfoAsync(request.Id);
    }
}
