using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Payments.ConfirmWithdrawalRequest
{
    public class ConfirmWithdrawalRequestCommand : IRequest
    {
        [JsonIgnore]
        public Guid WithdrawalRequestId { get; set; }
    }
}