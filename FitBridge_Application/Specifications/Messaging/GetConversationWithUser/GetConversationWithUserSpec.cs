using FitBridge_Domain.Entities.MessageAndReview;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Messaging.GetConversationWithUser
{
    public class GetConversationWithUserSpec : BaseSpecification<Conversation>
    {
        public GetConversationWithUserSpec(Guid targetUserId, Guid senderId) : base(x =>
            !x.IsGroup &&
            x.ConversationMembers.Count == 2 &&
            x.ConversationMembers.Any(cm => cm.UserId == senderId) &&
            x.ConversationMembers.Any(cm => cm.UserId == targetUserId))
        {
            AddInclude(c => c.ConversationMembers);
            AddInclude($"{nameof(Conversation.ConversationMembers)}.{nameof(ConversationMember.User)}");
            AddInclude(c => c.Messages);
        }
    }
}