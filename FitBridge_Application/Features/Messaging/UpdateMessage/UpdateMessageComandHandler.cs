using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications.Messaging.GetOtherConversationMembers;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.UpdateMessage
{
    internal class UpdateMessageComandHandler(
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        MessagingService messagingService,
        IMessagingHubService messagingHubService,
        IUnitOfWork unitOfWork) : IRequestHandler<UpdateMessageComand>
    {
        public async Task Handle(UpdateMessageComand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var senderId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var existingMessage = await unitOfWork.Repository<Message>().GetByIdAsync(request.MessageId)
                ?? throw new NotFoundException(nameof(Message));
            var existingConvo = await unitOfWork.Repository<Conversation>().GetByIdAsync(request.ConversationId)
                ?? throw new NotFoundException(nameof(Conversation));

            existingMessage.Content = request.NewContent;
            existingMessage.UpdatedAt = now;
            unitOfWork.Repository<Message>().Update(existingMessage);

            var index = await messagingService.GetMessageIndexAsync(request.ConversationId, request.MessageId);
            if (index == 1)
            {
                existingConvo.LastMessageContent = request.NewContent;
                existingConvo.UpdatedAt = now;
                unitOfWork.Repository<Conversation>().Update(existingConvo);
            }

            await unitOfWork.CommitAsync();

            var spec = new GetOtherConversationMembersSpec(request.ConversationId, senderId);
            var users = (await unitOfWork.Repository<ConversationMember>().GetAllWithSpecificationAsync(spec))
                .Select(x => x.UserId.ToString()).ToList();

            var dto = new MessageUpdatedDto
            {
                Id = request.MessageId,
                ConversationId = request.ConversationId,
                MessageType = MessageType.User.ToString(),
                NewContent = request.NewContent,
                Status = "Updated",
            };

            await messagingHubService.NotifyUsers(dto, users);
        }
    }
}