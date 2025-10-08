using MediatR;

namespace FitBridge_Application.Features.Notifications.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}