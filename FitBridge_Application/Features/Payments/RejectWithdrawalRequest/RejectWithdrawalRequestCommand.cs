using MediatR;
using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Features.Payments.RejectWithdrawalRequest
{
    public class RejectWithdrawalRequestCommand : IRequest
    {
        public Guid WithdrawalRequestId { get; set; }

        public string Reason { get; set; }
    }
}