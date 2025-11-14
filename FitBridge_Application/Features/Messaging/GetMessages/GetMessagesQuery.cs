using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Specifications.Messaging.GetMessages;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Messaging.GetMessages
{
    public class GetMessagesQuery : IRequest<IEnumerable<GetMessagesDto>>
    {
        [JsonIgnore]
        public Guid ConversationId { get; set; }

        public GetMessagesParam Params { get; set; }
    }
}