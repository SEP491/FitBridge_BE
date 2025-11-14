using FitBridge_Application.Dtos.Messaging;

namespace FitBridge_Application.Interfaces.Services.Messaging
{
    public interface IMessagingHubService
    {
        public Task NotifyUsers(IMessagingHubDto dto, IEnumerable<string> userIds);
    }
}