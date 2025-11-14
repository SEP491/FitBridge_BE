using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Messaging.UpdateMessage
{
    public class UpdateMessageComand : IRequest
    {
        [JsonIgnore]
        public Guid MessageId { get; set; }

        public Guid ConversationId { get; set; }

        public string NewContent { get; set; }
    }
}