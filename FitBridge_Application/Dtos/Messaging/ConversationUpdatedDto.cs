namespace FitBridge_Application.Dtos.Messaging;

public class ConversationUpdatedDto : IMessagingHubDto
{
    public Guid ConversationId { get; set; }

    public string? Title { get; set; }

    public string? ConversationImage { get; set; }

    public DateTime UpdatedAt { get; set; }
}
