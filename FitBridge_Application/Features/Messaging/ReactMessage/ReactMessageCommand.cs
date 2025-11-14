using MediatR;

namespace FitBridge_Application.Features.Messaging.ReactMessage
{
    public class ReactMessageCommand : IRequest
    {
        public Guid MessageId { get; set; }

        public Guid ConversationId { get; set; }

        public string? Reaction { get; set; }

        public bool RemoveReaction { get; set; } = false;
    }
}