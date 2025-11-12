using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class ConversationMember : BaseEntity
{
    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; }

    public string CustomTitle { get; set; }

    public string ConversationImage { get; set; }

    public DateTime? LastReadAt { get; set; }

    public Guid ConversationId { get; set; }

    public Conversation Conversation { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();

    public Guid? LastMessageId { get; set; }

    public Message? LastReadMessage { get; set; }
}