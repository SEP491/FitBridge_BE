using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Services.Messaging;
using Microsoft.AspNetCore.SignalR;

namespace FitBridge_Infrastructure.Services.Messaging
{
    internal class MessagingHubService(IHubContext<MessagingHub, IMessagingClient> hubContext) : IMessagingHubService
    {
        public Task NotifyUsers(IMessagingHubDto dto, IEnumerable<string> userIds)
        {
            return dto switch
            {
                MessageReceivedDto messageReceivedDto => hubContext.Clients.Users(userIds).MessageReceived(messageReceivedDto),
                MessageUpdatedDto messageUpdatedDto => hubContext.Clients.Users(userIds).MessageUpdated(messageUpdatedDto),
                ReactionReceivedDto reactionReceivedDto => hubContext.Clients.Users(userIds).ReactionReceived(reactionReceivedDto),
                ReactionRemovedDto reactionRemovedDto => hubContext.Clients.Users(userIds).ReactionRemoved(reactionRemovedDto),
                UpdateMessageStatusDto updateMessageStatusDto => hubContext.Clients.Users(userIds).UpdateMessageStatus(updateMessageStatusDto),
                UserTypingDto userTypingDto => hubContext.Clients.Users(userIds).UserTyping(userTypingDto),
                UserPresenceUpdateDto userPresenceUpdateDto => hubContext.Clients.Users(userIds).UserPresenceUpdate(userPresenceUpdateDto),
                _ => Task.CompletedTask,
            };
        }
    }
}