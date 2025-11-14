using FitBridge_Application.Dtos.Messaging;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Messaging.GetConversationWithUser
{
    public class GetConversationWithUserQuery : IRequest<GetConversationWithUserResponse>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}