using FitBridge_Application.Dtos.Templates;
using FitBridge_Domain.Enums.MessageAndReview;
using System.Text.Json;

namespace FitBridge_Application.Dtos.Notifications
{
    public class NotificationMessage(EnumContentType notificationType,
                                     List<Guid> userIds,
                                     IBaseTemplateModel model,
                                     string? payload = "{}")
    {
        public EnumContentType NotificationTypes { get; set; } = notificationType;

        public List<Guid> UserIds { get; set; } = userIds;

        public string NotificationPayload { get; set; } = payload;

        public NotificationDto? NotificationDto { get; set; } = null;

        public TemplateDto? InAppNotificationTemplate { get; set; }

        public TemplateDto? PushNotificationTemplate { get; set; }

        public IBaseTemplateModel TemplateModel { get; set; } = model;
    }
}