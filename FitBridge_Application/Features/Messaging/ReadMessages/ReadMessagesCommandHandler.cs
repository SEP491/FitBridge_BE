using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetMessages;
using FitBridge_Application.Specifications.Messaging.GetMessageStatuses;
using FitBridge_Application.Specifications.Messaging.GetOtherConversationMembers;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.ReadMessages
{
    internal class ReadMessagesCommandHandler(
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IMessagingHubService messagingHubService) : IRequestHandler<ReadMessagesCommand>
    {
        public async Task Handle(ReadMessagesCommand request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(ApplicationUser));

            // Validate that all messages belong to the specified conversation
            var messageIds = request.MessageIds;
            var msgSpec = new GetMessagesSpec(request.ConversationId, messagesInclude: request.MessageIds);
            var messages = await unitOfWork.Repository<Message>().GetAllWithSpecificationAsync(msgSpec);

            if (messages.Count != messageIds.Count)
            {
                throw new InvalidDataException("Some messages not found or don't belong to this conversation.");
            }

            var now = DateTime.UtcNow;
            var statusesToAdd = new List<MessageStatus>();

            foreach (var messageId in messageIds)
            {
                var msgStatusSpec = new GetMessageStatusesSpec(messageId, userId);
                var existingStatus = await unitOfWork.Repository<MessageStatus>()
                    .GetBySpecificationAsync(msgStatusSpec);

                if (existingStatus == null)
                {
                    var newStatus = new MessageStatus
                    {
                        MessageId = messageId,
                        UserId = userId,
                        CurrentStatus = CurrentMessageStatus.Read,
                        ReadAt = now,
                        DeliveredAt = now,
                        SentAt = null
                    };
                    statusesToAdd.Add(newStatus);
                }
                else if (existingStatus.ReadAt == null)
                {
                    existingStatus.ReadAt = now;
                    if (existingStatus.DeliveredAt == null)
                    {
                        existingStatus.DeliveredAt = now;
                    }
                    unitOfWork.Repository<MessageStatus>().Update(existingStatus);
                }
            }
            if (statusesToAdd.Any())
            {
                unitOfWork.Repository<MessageStatus>().InsertRange(statusesToAdd);
            }
            await unitOfWork.CommitAsync();

            var spec = new GetOtherConversationMembersSpec(request.ConversationId, userId);
            var users = (await unitOfWork.Repository<ConversationMember>().GetAllWithSpecificationAsync(spec))
                .Select(x => x.UserId.ToString()).ToList();

            if (users.Count > 0)
            {
                var statusUpdate = new UpdateMessageStatusDto
                {
                    UpdatedStatus = "read",
                    Timestamp = now
                };
                await messagingHubService.NotifyUsers(statusUpdate, users);
            }
        }
    }
}