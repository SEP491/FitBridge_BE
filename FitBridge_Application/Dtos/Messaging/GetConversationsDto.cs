namespace FitBridge_Application.Dtos.Messaging;

public class GetConversationsDto
{
    public Guid Id { get; set; }

    public bool IsGroup { get; set; }

    public string Title { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string LastMessageContent { get; set; }

    public string LastMessageType { get; set; }

    public string LastMessageMediaType { get; set; }

    public string? LastMessageSenderName { get; set; }

    public Guid? LastMessageSenderId { get; set; } // to help client compare with their own current id

    public bool IsRead { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastActiveAt { get; set; }

    public string ConversationImg { get; set; }

    public IEnumerable<GetConversationMembersDto> Members { get; set; }
}

public class GetConversationMembersDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; }

    public string ImgUrl { get; set; }

    public string Role { get; set; }
}