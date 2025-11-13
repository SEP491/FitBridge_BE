namespace FitBridge_Application.Dtos.Messaging;

public class GetMessagesDto
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string MediaType { get; set; }

    public string MessageType { get; set; }

    public Guid ConversationId { get; set; }

    public string DeliveryStatus { get; set; }

    public string? Status { get; set; }

    public Guid? ReplyToMessageId { get; set; }

    public string? ReplyToMessageContent { get; set; }

    public string? ReplyToMessageMediaType { get; set; }

    public Guid? SenderId { get; set; }

    public string Reaction { get; set; }

    public string? SenderName { get; set; }

    public string? SenderAvatarUrl { get; set; }

    public BookingRequestDto? BookingRequest { get; set; }
}
