namespace FitBridge_Application.Specifications.Messaging.GetAllUsersForMessaging;

public class GetAllUsersForMessagingParam : BaseParams
{
    /// <summary>
    /// Optional role filter to include only users with specific roles.
    /// If empty or null, no role filtering is applied (but excludes current user's role in handler).
    /// </summary>
    public List<string>? RoleFilter { get; set; }
}