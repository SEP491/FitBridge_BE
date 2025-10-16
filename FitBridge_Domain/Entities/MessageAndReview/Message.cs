using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Message : BaseEntity
{
    public string Content { get; set; }

    public MessageType MessageType { get; set; }

    public string Metadata { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; }

    public ConversationMember Sender { get; set; }
    public bool IsEdited { get; set; }

    public Guid SenderId { get; set; }

    public Guid? ReplyToMessageId { get; set; }

    public ICollection<Message> ReplyToMessages { get; set; } = new List<Message>();

    public Message? ReplyToMessage { get; set; }

    public MessageStatus MessageStatus { get; set; }
}