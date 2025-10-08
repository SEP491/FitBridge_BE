using MediatR;

namespace FitBridge_Application.Features.Notifications.DeleteNotification
{
    public class DeleteNotificationCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}