using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Messaging.GetConversations;

public class GetConversationsSpec : BaseSpecification<Conversation>
{
    public GetConversationsSpec(Guid userId, GetConversationsParam parameter)
        : base(c => c.ConversationMembers.Any(cm => cm.UserId == userId) && c.IsEnabled)
    {
        AddInclude(c => c.ConversationMembers);
        AddInclude($"{nameof(Conversation.ConversationMembers)}.{nameof(ConversationMember.User)}");
        AddInclude(c => c.Messages);

        AddOrderByDesc(c => c.UpdatedAt);

        if (parameter.DoApplyPaging)
        {
            AddPaging((parameter.Page - 1) * parameter.Size, parameter.Size);
        }
    }
}