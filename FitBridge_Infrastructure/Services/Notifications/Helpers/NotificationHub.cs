using AutoMapper;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Services.Notifications.UserNotifications;
using FitBridge_Infrastructure.Services.Notifications.Enums;
using Microsoft.AspNetCore.SignalR;

namespace FitBridge_Infrastructure.Services.Notifications.Helpers
{
    internal class NotificationHub(
        NotificationConnectionManager notificationConnectionManager) : Hub<IUserNotifications>
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

        public async Task AddToGroup(NotificationGroups groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName.ToString());
        }

        public async Task RemoveFromGroup(NotificationGroups groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName.ToString());
        }
    }
}