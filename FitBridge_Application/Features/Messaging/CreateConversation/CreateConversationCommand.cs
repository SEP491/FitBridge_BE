using FitBridge_Application.Dtos.Messaging;
using MediatR;

namespace FitBridge_Application.Features.Messaging.CreateConversation
{
    public class CreateConversationCommand : IRequest<CreateConversationResponse>
    {
        public bool IsGroup { get; set; }

        public List<CreateConversationMemberCommand> Members { get; set; }

        public string NewMessageContent { get; set; }

        public string? GroupImage { get; set; }
    }

    public class CreateConversationMemberCommand
    {
        public Guid MemberId { get; set; }

        public string MemberName { get; set; }

        public string MemberAvatarUrl { get; set; }
    }
}