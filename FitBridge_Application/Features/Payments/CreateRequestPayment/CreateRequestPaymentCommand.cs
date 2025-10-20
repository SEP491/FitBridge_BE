using FitBridge_Application.Dtos.Payments;
using MediatR;

namespace FitBridge_Application.Features.Payments.CreateRequestPayment
{
    public class CreateRequestPaymentCommand : IRequest<RequestPaymentResponseDto>
    {
        public decimal Amount { get; set; }

        public string Note { get; set; }

        public string BankName { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public string ImageUrl { get; set; }
    }
}