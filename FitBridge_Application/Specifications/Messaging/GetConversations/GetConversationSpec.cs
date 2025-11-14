using FitBridge_Domain.Entities.MessageAndReview;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Messaging.GetConversations
{
    public class GetConversationSpec : BaseSpecification<Conversation>
    {
        public GetConversationSpec(bool isGroup, List<Guid> membersIds) : base(x =>
            x.IsGroup == isGroup
            && x.ConversationMembers.Count == membersIds.Count
            && x.ConversationMembers.All(cm => membersIds.Contains(cm.UserId)))
        {
            AddInclude(x => x.ConversationMembers);
        }
    }
}