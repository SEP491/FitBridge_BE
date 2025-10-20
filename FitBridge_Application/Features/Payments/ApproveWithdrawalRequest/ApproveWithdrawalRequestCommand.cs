using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Payments.ApproveWithdrawalRequest
{
    public class ApproveWithdrawalRequestCommand : IRequest
    {
        [JsonIgnore]
        public Guid WithdrawalRequestId { get; set; }
    }
}