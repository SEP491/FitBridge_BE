using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetConversationWithUser;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.GetConversationWithUser
{
    internal class GetConversationWithUserQueryHandler(
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetConversationWithUserQuery, GetConversationWithUserResponse>
    {
        public async Task<GetConversationWithUserResponse> Handle(GetConversationWithUserQuery request, CancellationToken cancellationToken)
        {
            var senderId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(ApplicationUser));

            var convoSpec = new GetConversationWithUserSpec(request.UserId, senderId);
            var conversation = await unitOfWork.Repository<Conversation>().GetBySpecificationAsync(convoSpec);
            if (conversation == null)
            {
                return new GetConversationWithUserResponse
                {
                    ConversationId = null,
                    Members = []
                };
            }

            var members = conversation.ConversationMembers
                .Select(cm => new MemberDto(
                    cm.UserId,
                    cm.User?.FullName ?? string.Empty,
                    cm.ConversationImage ?? string.Empty
                ))
                .ToList();

            return new GetConversationWithUserResponse
            {
                ConversationId = conversation.Id,
                Members = members
            };
        }
    }
}