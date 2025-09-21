using System;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Template : BaseEntity
{
    public EnumContentType ContentType { get; set; }
    public string TemplateBody { get; set; } = null!;
    public ICollection<PushNotificationTokens> PushNotificationTokens { get; set; } = new List<PushNotificationTokens>();
    public InAppNotification InAppNotification { get; set; } = new InAppNotification();
}

public enum EnumContentType
{
    PushNotification,
    InAppNotification
}
