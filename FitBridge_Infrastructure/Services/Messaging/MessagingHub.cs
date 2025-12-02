using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Services.Messaging
{
    public class MessagingHub(
        ILogger<MessagingHub> logger,
        IApplicationUserService applicationUserService) : Hub<IMessagingClient>
    {
        public override async Task OnConnectedAsync()
        {
            ArgumentException.ThrowIfNullOrEmpty(Context.UserIdentifier);
            logger.LogInformation("User {User} connected with ConnectionId {ConnectionId}",
                Context.UserIdentifier, Context.ConnectionId);

            Guid.TryParse(Context.UserIdentifier, out var userId);
            await applicationUserService.UpdateUserPresence(userId, isOnline: true);

            var dto = new UserPresenceUpdateDto
            {
                UserId = userId,
                IsOnline = true
            };
            await Clients.Others.UserPresenceUpdate(dto);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ArgumentException.ThrowIfNullOrEmpty(Context.UserIdentifier);
            logger.LogInformation("User {User} disconnected with ConnectionId {ConnectionId}",
                Context.UserIdentifier, Context.ConnectionId);

            Guid.TryParse(Context.UserIdentifier, out var userId);
            await applicationUserService.UpdateUserPresence(userId, isOnline: true);

            var dto = new UserPresenceUpdateDto
            {
                UserId = userId,
                IsOnline = false
            };
            await Clients.Others.UserPresenceUpdate(dto);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<bool> UserTyping(UserTypingDto typing)
        {
            ArgumentException.ThrowIfNullOrEmpty(Context.UserIdentifier);
            if (typing.IsTyping)
            {
                logger.LogInformation("User {User} is typing in conversation {ConversationId}",
                    Context.UserIdentifier, typing.ConversationId);
            }
            else
            {
                logger.LogInformation("User {User} is typing in conversation {ConversationId}",
                    Context.UserIdentifier, typing.ConversationId);
            }

            logger.LogInformation("User {User} is typing in conversation {ConversationId}",
                Context.UserIdentifier, typing.ConversationId);
            await Clients.OthersInGroup(typing.ConversationId.ToString()).UserTyping(typing); // no need clients ack since not critical
            return true;
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            logger.LogInformation("User {User} joined group {GroupName}", Context.UserIdentifier, groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            logger.LogInformation("User {User} left group {GroupName}", Context.UserIdentifier, groupName);
        }

        //public async Task Acknowledge(MessageDecorator<BaseDecorator> decorator)
        //{
        //    ArgumentException.ThrowIfNullOrEmpty(Context.UserIdentifier);
        //    logger.LogInformation("User {User} acknowledged message with Id {MessageId}",
        //        Context.UserIdentifier, decorator.Id);
        //}
    }
}