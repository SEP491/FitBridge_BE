using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Conversation : BaseEntity
{
    public bool IsGroup { get; set; }

    public Guid? LastMessageId { get; set; }

    public string LastMessageContent { get; set; }

    public MessageType LastMessageType { get; set; }

    public MediaType LastMessageMediaType { get; set; }

    public string? LastMessageSenderName { get; set; }

    public Guid? LastMessageSenderId { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
}