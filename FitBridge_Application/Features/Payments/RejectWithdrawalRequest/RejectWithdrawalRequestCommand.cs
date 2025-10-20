using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Payments.RejectWithdrawalRequest
{
    public class RejectWithdrawalRequestCommand : IRequest
    {
        [JsonIgnore]
        public Guid WithdrawalRequestId { get; set; }

        public string Reason { get; set; }
    }
}