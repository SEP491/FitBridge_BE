using FitBridge_Application.Dtos.Messaging;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Messaging.GetMessagesInRange
{
    public class GetMessagesInRangeQuery : IRequest<IEnumerable<GetMessagesDto>>
    {
        [JsonIgnore]
        public Guid ConversationId { get; set; }

        public int CurrentPage { get; set; }

        public Guid TargetMessageId { get; set; }
    }
}