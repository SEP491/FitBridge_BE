using MediatR;

namespace FitBridge_Application.Features.Messaging.ReadMessages
{
    public class ReadMessagesCommand : IRequest
    {
        public Guid ConversationId { get; set; }

        public List<Guid> MessageIds { get; set; }
    }
}