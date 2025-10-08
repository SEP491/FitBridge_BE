using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Specifications.Notifications.GetNotificationsByUserId;
using MediatR;

namespace FitBridge_Application.Features.Notifications.GetUserNotifications
{
    public class GetUserNotificationsQuery(GetNotificationsByUserIdParams parameters) : IRequest<PagingResultDto<NotificationDto>>
    {
        public GetNotificationsByUserIdParams Parameters { get; set; } = parameters;
    }
}