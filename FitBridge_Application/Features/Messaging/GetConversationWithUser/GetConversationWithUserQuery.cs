using FitBridge_Application.Dtos.Messaging;
using MediatR;

namespace FitBridge_Application.Features.Messaging.GetConversationWithUser
{
    public class GetConversationWithUserQuery : IRequest<GetConversationWithUserResponse>
    {
        public Guid UserId { get; set; }
    }
}