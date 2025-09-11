using System;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Message : BaseEntity
{
    public string Content { get; set; }
    public MessageType MessageType { get; set; }
    public string Metadata { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid ConversationId { get; set; }
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    public Conversation Conversation { get; set; }
    public ConversationMember Sender { get; set; }
    public ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
    public Guid SenderId { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public ICollection<Message> ReplyToMessages { get; set; } = new List<Message>();
    public Message? ReplyToMessage { get; set; }
    public MessageStatus MessageStatus { get; set; }
}

public enum MessageType
{

}
