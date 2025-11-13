using FitBridge_Application.Dtos.Messaging;
using MediatR;

namespace FitBridge_Application.Features.Messaging.GetMessagesInRange
{
    public class GetMessagesInRangeQuery : IRequest<IEnumerable<GetMessagesDto>>
    {
        public Guid ConversationId { get; set; }

        public int CurrentPage { get; set; }

        public Guid TargetMessageId { get; set; }
    }
}