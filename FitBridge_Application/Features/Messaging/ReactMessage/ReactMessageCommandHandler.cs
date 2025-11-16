using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetConversationMembers;
using FitBridge_Application.Specifications.Messaging.GetOtherConversationMembers;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitBridge_Application.Features.Messaging.ReactMessage
{
    internal class ReactMessageCommandHandler(
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IMessagingHubService messagingHubService,
        ILogger<ReactMessageCommandHandler> logger,
        IUnitOfWork unitOfWork) : IRequestHandler<ReactMessageCommand>
    {
        public async Task Handle(ReactMessageCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var senderId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                     ?? throw new NotFoundException(nameof(ApplicationUser));

            var message = await unitOfWork.Repository<Message>().GetByIdAsync(
                request.MessageId, includes: [nameof(Message.Conversation)]);

            if (message == null)
            {
                throw new NotFoundException(nameof(Message));
            }

            // Verify user is part of the conversation
            var convoMemberSpec = new GetConversationMembersSpec(message.ConversationId, senderId);
            var isMember = await unitOfWork.Repository<ConversationMember>().AnyAsync(convoMemberSpec);

            if (!isMember)
            {
                throw new DataValidationFailedException("Not a member of the conversation");
            }

            if (request.RemoveReaction)
            {
                message.Reaction = null;
            }
            else
            {
                message.Reaction = request.Reaction;
            }

            message.UpdatedAt = now;
            unitOfWork.Repository<Message>().Update(message);

            await unitOfWork.CommitAsync();

            // Notify other users in the conversation
            var spec = new GetOtherConversationMembersSpec(request.ConversationId, senderId);
            var users = (await unitOfWork.Repository<ConversationMember>().GetAllWithSpecificationAsync(spec))
                .Select(x => x.UserId.ToString()).ToList();

            if (users.Count > 0)
            {
                if (!request.RemoveReaction)
                {
                    var reactionDto = new ReactionReceivedDto
                    {
                        MessageId = request.MessageId,
                        Reaction = request.Reaction
                    };
                    await messagingHubService.NotifyUsers(reactionDto, users);
                }
                else
                {
                    var reactionDto = new ReactionRemovedDto
                    {
                        MessageId = request.MessageId,
                    };
                    await messagingHubService.NotifyUsers(reactionDto, users);
                }
            }
            else
            {
                logger.LogInformation("No users to identify");
            }
        }
    }
}