using MediatR;

namespace FitBridge_Application.Features.Payments.ApproveWithdrawalRequest
{
    public class ApproveWithdrawalRequestCommand : IRequest
    {
        public Guid WithdrawalRequestId { get; set; }
    }
}
