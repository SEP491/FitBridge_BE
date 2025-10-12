using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Interfaces.Services.Notifications.UserNotifications;
using FitBridge_Infrastructure.Services.Notifications.Enums;
using FitBridge_Infrastructure.Services.Notifications.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Services.Notifications
{
    [Authorize]
    public class NotificationHub(
        NotificationConnectionManager notificationConnectionManager,
        NotificationHandshakeManager notificationHandshakeManager,
        ILogger<NotificationHub> logger) : Hub<IUserNotifications>
    {
        public override async Task OnConnectedAsync()
        {
            ArgumentException.ThrowIfNullOrEmpty(Context.UserIdentifier);
            await notificationConnectionManager.AddConnectionAsync(Context.UserIdentifier, Context.ConnectionId);
            logger.LogInformation("User {User} connected with ConnectionId {ConnectionId}",
                Context.UserIdentifier, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ArgumentException.ThrowIfNullOrEmpty(Context.UserIdentifier);
            await notificationConnectionManager.RemoveConnectionAsync(Context.UserIdentifier);
            logger.LogInformation("User {User} disconnected with ConnectionId {ConnectionId}",
                Context.UserIdentifier, Context.ConnectionId);
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

        public async Task ConfirmHandshake()
        {
            logger.LogInformation("Handshake {User} from {ConnectionId} invoked",
                Context.UserIdentifier, Context.ConnectionId);
            var userId = Context.UserIdentifier;
            await notificationHandshakeManager.ConfirmHandshake(userId);
        }
    }
}