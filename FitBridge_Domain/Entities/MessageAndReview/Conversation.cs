using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Conversation : BaseEntity
{
    public string Title { get; set; }
    public Guid? LastMessageId { get; set; }
    public string LastMessageContent { get; set; }
    public LastMessageType? LastMessageType { get; set; }
    public Guid? LastMessageSenderId { get; set; }
    public Message LastMessage { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();
    public ConversationMember? LastMessageSender { get; set; }
}

public enum LastMessageType
{

}
