using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Messaging.GetOtherConversationMembers
{
    public class GetOtherConversationMembersSpec : BaseSpecification<ConversationMember>
    {
        public GetOtherConversationMembersSpec(Guid conversationId, Guid userId) : base(x =>
            x.ConversationId == conversationId
            && x.UserId != userId)
        {
        }
    }
}