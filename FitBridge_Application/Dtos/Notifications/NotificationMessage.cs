using FitBridge_Application.Dtos.Templates;
using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Application.Dtos.Notifications
{
    public class NotificationMessage(EnumContentType notificationType, List<Guid> userIds, IBaseTemplateModel model, Dictionary<string, string>? payload = null)
    {
        public EnumContentType NotificationTypes { get; set; } = notificationType;

        public List<Guid> UserIds { get; set; } = userIds;

        public Dictionary<string, string>? NotificationPayload { get; set; } = payload;

        public NotificationDto? NotificationDto { get; set; } = null;

        public TemplateDto? InAppNotificationTemplate { get; set; }

        public TemplateDto? PushNotificationTemplate { get; set; }

        public IBaseTemplateModel TemplateModel { get; set; } = model;
    }
}