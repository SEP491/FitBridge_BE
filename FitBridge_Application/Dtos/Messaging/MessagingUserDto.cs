namespace FitBridge_Application.Dtos.Messaging;

public class MessagingUserDto
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public bool IsMale { get; set; }

    public string AvatarUrl { get; set; }

    public string UserRole { get; set; }
}
