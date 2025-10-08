using FitBridge_Application.Dtos.Notifications;
using MediatR;

namespace FitBridge_Application.Features.Notifications.GetUnreadCount
{
    public class GetUnreadNotificationCountQuery : IRequest<UnreadNotificationCountDto>
    {
    }
}