using FitBridge_Application.Dtos.Payments;
using MediatR;

namespace FitBridge_Application.Features.Payments.CreatePaymentLink;

public class CreatePaymentLinkCommand : IRequest<PaymentResponseDto>
{   
    public CreatePaymentRequestDto Request { get; set; }
}
