namespace FitBridge_Application.Dtos.Messaging
{
    public class GetConversationWithUserResponse
    {
        public Guid? ConversationId { get; set; }

        public List<MemberDto> Members { get; set; }
    }

    public record MemberDto(Guid UserId, string username, string avatarUrl);
}