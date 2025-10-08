using MediatR;
using System;

namespace FitBridge_Application.Features.Notifications.DismissNotification
{
    public class DismissNotificationCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}