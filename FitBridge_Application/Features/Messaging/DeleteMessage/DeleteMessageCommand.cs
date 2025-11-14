using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Messaging.DeleteMessage
{
    public class DeleteMessageCommand : IRequest
    {
        [JsonIgnore]
        public Guid MessageId { get; set; }
    }
}