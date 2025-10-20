using MediatR;

namespace FitBridge_Application.Features.Payments.ConfirmWithdrawalRequest
{
    public class ConfirmWithdrawalRequestCommand : IRequest
    {
        public Guid WithdrawalRequestId { get; set; }

        public string ImageUrl { get; set; }
    }
}