using MediatR;

namespace FitBridge_Application.Features.Messaging.UpdateMessage
{
    public class UpdateMessageComand : IRequest
    {
        public Guid MessageId { get; set; }

        public Guid ConversationId { get; set; }

        public string NewContent { get; set; }
    }
}