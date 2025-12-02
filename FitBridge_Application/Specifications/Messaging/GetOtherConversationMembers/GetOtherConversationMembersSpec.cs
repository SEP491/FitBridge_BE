using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Messaging.GetOtherConversationMembers
{
    public class GetOtherConversationMembersSpec : BaseSpecification<ConversationMember>
    {
        public GetOtherConversationMembersSpec(Guid conversationId, Guid userId, bool includeUser = false) : base(x =>
            x.ConversationId == conversationId
            && x.UserId != userId)
        {
            if (includeUser)
            {
                AddInclude(x => x.User);
            }
        }
    }
}