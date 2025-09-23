using AutoMapper;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Services.Notifications.UserNotifications;
using Microsoft.AspNetCore.SignalR;

namespace FitBridge_Infrastructure.Services.Notifications
{
    internal class NotificationHub(
        INotificationConnectionManager notificationConnectionManager,
        IUnitOfWork unitOfWork,
        IMapper mapper) : Hub<IUserNotifications>
    {
        public override async Task OnConnectedAsync()
        {
            ArgumentException.ThrowIfNullOrEmpty(Context.UserIdentifier);
            await notificationConnectionManager.AddConnectionAsync(Context.UserIdentifier, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ArgumentException.ThrowIfNullOrEmpty(Context.UserIdentifier);
            await notificationConnectionManager.RemoveConnectionAsync(Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<List<NotificationDto>> GetStoredNotifications(Guid userId)
        {
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}