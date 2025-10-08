using FitBridge_Domain.Enums.Notifications;
using MediatR;

namespace FitBridge_Application.Features.Notifications.AddUserDeviceToken
{
    public class AddUserDeviceTokenCommand : IRequest
    {
        public string DeviceToken { get; set; }

        public PlatformEnum Platform { get; set; }
    }
}