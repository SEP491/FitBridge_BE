using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Template : BaseEntity
{
    public EnumContentType ContentType { get; set; }

    public string TemplateBody { get; set; } = null!;

    public ICollection<PushNotificationTokens> PushNotificationTokens { get; set; } = new List<PushNotificationTokens>();

    public Notification InAppNotification { get; set; } = new Notification();
}