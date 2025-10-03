using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Notifications;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class PushNotificationTokens : BaseEntity
{
    public string DeviceToken { get; set; }

    public Guid UserId { get; set; }
    public PlatformEnum Platform { get; set; } // e.g., iOS, Android, Web

    public ApplicationUser User { get; set; }
}