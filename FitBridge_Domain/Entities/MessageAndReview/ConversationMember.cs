using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class ConversationMember : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string CustomTitle { get; set; }
    public string ConversationImage { get; set; }
    public Guid? LastMessageId { get; set; }
    public DateTime? LastReadAt { get; set; }
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    public Conversation Conversation { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
    public Message? LastMessage { get; set; }
}
