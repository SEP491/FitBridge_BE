using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetOtherConversationMembers;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.DeleteMessage
{
    internal class DeleteMessageCommandHandler(
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IMessagingHubService messagingHubService) : IRequestHandler<DeleteMessageCommand>
    {
        public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var senderId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(ApplicationUser));

            var existingMessage = await unitOfWork.Repository<Message>().GetByIdAsync(request.MessageId, includes: [nameof(Message.Conversation)])
                ?? throw new NotFoundException(nameof(Message));
            if (existingMessage.SenderId != senderId)
            {
                throw new InvalidDataException("Sender not match with database");
            }

            existingMessage.DeletedAt = now;
            unitOfWork.Repository<Message>().Update(existingMessage);

            // Update conversation's last message content if this was the last message
            var conversation = existingMessage.Conversation;
            if (conversation.LastMessageId == request.MessageId)
            {
                conversation.UpdatedAt = now;
                unitOfWork.Repository<Conversation>().Update(conversation);
            }

            await unitOfWork.CommitAsync();

            var spec = new GetOtherConversationMembersSpec(existingMessage.ConversationId, senderId);
            var users = (await unitOfWork.Repository<ConversationMember>().GetAllWithSpecificationAsync(spec))
                .Select(x => x.UserId.ToString()).ToList();

            if (users.Count > 0)
            {
                var dto = new MessageUpdatedDto
                {
                    Id = request.MessageId,
                    ConversationId = existingMessage.ConversationId,
                    MessageType = MessageType.User.ToString(),
                    NewContent = existingMessage.Content,
                    Status = "Deleted",
                };

                await messagingHubService.NotifyUsers(dto, users);
            }
        }
    }
}