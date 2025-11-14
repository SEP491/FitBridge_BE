using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class MessageStatus : BaseEntity
{
    public Guid MessageId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public CurrentMessageStatus CurrentStatus { get; set; }

    public DateTime? ReadAt { get; set; }

    public Message Message { get; set; }

    public ConversationMember User { get; set; }
}