using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Specifications.Messaging.GetMessages;
using MediatR;

namespace FitBridge_Application.Features.Messaging.GetMessages
{
    public class GetMessagesQuery : IRequest<IEnumerable<GetMessagesDto>>
    {
        public Guid ConversationId { get; set; }

        public GetMessagesParam Params { get; set; }
    }
}