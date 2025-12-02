using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Messaging.GetConversations;

public class GetConversationsSpec : BaseSpecification<Conversation>
{
    public GetConversationsSpec(Guid userId, GetConversationsParam parameter)
        : base(c =>
        c.ConversationMembers.Any(cm => cm.UserId == userId) && c.IsEnabled
        && (string.IsNullOrEmpty(parameter.SearchTerm) ||
            c.ConversationMembers.First(cm => cm.UserId == userId).CustomTitle
                                 .ToLower()
                                 .Contains(parameter.SearchTerm.ToLower())))
    {
        AddInclude(c => c.ConversationMembers);
        AddInclude($"{nameof(Conversation.ConversationMembers)}.{nameof(ConversationMember.User)}");
        AddInclude(c => c.Messages);
        AddInclude($"{nameof(Conversation.Messages)}.{nameof(Message.MessageStatuses)}");

        AddOrderByDesc(c => c.UpdatedAt);

        if (parameter.DoApplyPaging)
        {
            AddPaging((parameter.Page - 1) * parameter.Size, parameter.Size);
        }
    }
}