using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class PushNotificationTokens : BaseEntity
{
    public string DeviceToken { get; set; }
    public Guid UserId { get; set; }
    public Guid TemplateId { get; set; }
    public Template Template { get; set; }
    public ApplicationUser User { get; set; }
}
