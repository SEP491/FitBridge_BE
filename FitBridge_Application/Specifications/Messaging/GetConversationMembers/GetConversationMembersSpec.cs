using FitBridge_Domain.Entities.MessageAndReview;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Messaging.GetConversationMembers
{
    public class GetConversationMembersSpec : BaseSpecification<ConversationMember>
    {
        public GetConversationMembersSpec(Guid convoId, Guid? userId = null) : base(x =>
            x.ConversationId == convoId
            && (userId == null || x.UserId == userId))
        {
            AddInclude(x => x.User);
        }
    }
}