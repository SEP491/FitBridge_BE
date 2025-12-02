using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetConversationMembers;
using FitBridge_Application.Specifications.Messaging.GetConversations;
using FitBridge_Application.Specifications.Messaging.GetOtherConversationMembers;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.GetConversations;

internal class GetConversationsQueryHandler(
    IUnitOfWork unitOfWork,
    IUserUtil userUtil,
    IApplicationUserService applicationUserService,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetConversationsQuery, PagingResultDto<GetConversationsDto>>
{
    public async Task<PagingResultDto<GetConversationsDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));
        var userRole = userUtil.GetUserRole(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException("User role not found");

        var spec = new GetConversationsSpec(userId, request.Params);
        var conversations = await unitOfWork.Repository<Conversation>()
            .GetAllWithSpecificationAsync(spec, asNoTracking: true);
        var totalCount = await unitOfWork.Repository<Conversation>().CountAsync(spec);

        var dtos = new List<GetConversationsDto>();

        //=======
        //            var isRead = false;
        //            if (x.LastMessageId.HasValue)
        //            {
        //                var lastMessage = x.Messages.FirstOrDefault(m => m.Id == x.LastMessageId.Value);
        //                if (lastMessage != null)
        //                {
        //                    var convoMember = await unitOfWork.Repository<ConversationMember>()
        //                        .GetBySpecificationAsync(new GetConversationMembersSpec(lastMessage.ConversationId, userId))
        //                        ?? throw new NotFoundException(nameof(ConversationMember));
        //                    var messageStatus = lastMessage.MessageStatuses.FirstOrDefault(ms => ms.UserId == convoMember.Id);

        //                    isRead = messageStatus?.ReadAt != null;
        //                }
        //            }

        //            var isActive = false;
        //            var convoMembers = await unitOfWork.Repository<ConversationMember>()
        //                .GetAllWithSpecificationAsync(new GetOtherConversationMembersSpec(x.Id, userId), asNoTracking: true);

        //            DateTime lastActiveAt = DateTime.UtcNow; // default to now if members are active
        //            var convoMemberUsers = convoMembers.Select(cm => cm.User).ToList();
        //            isActive = convoMemberUsers.Any(u => u.IsActive);
        //            if (!isActive && convoMemberUsers.Count > 0)
        //            {
        //                lastActiveAt = convoMemberUsers.Max(u => u.LastSeen);
        //            }

        //            dtos.Add(new GetConversationsDto
        //            {
        //                Id = x.Id,
        //                IsGroup = x.IsGroup,
        //                Title = x.ConversationMembers.First(cm => cm.UserId == userId).CustomTitle,
        //                UpdatedAt = x.UpdatedAt,
        //                LastMessageContent = x.LastMessageContent,
        //                LastMessageType = x.LastMessageType.ToString(),
        //                LastMessageMediaType = x.LastMessageMediaType.ToString(),
        //                LastMessageSenderName = x.LastMessageSenderName,
        //                LastMessageSenderId = x.LastMessageSenderId,
        //                IsRead = isRead,
        //                IsActive = isActive,
        //                LastActiveAt = lastActiveAt,
        //                ConversationImg = x.ConversationMembers.First(cm => cm.UserId == userId).ConversationImage,
        //                Members = members
        //            });
        //        }

        //        var orderedDtos = dtos.OrderByDescending(x => x.UpdatedAt).ToList();

        //        return new PagingResultDto<GetConversationsDto>(totalCount, orderedDtos);
        //>>>>>>> Stashed changes
        foreach (var x in conversations)
        {
            var members = new List<GetConversationMembersDto>();

            foreach (var cm in x.ConversationMembers)
            {
                var memberRole = await applicationUserService.GetUserRoleAsync(cm.User);
                members.Add(new GetConversationMembersDto
                {
                    UserId = cm.UserId,
                    Username = cm.User.UserName ?? string.Empty,
                    ImgUrl = cm.User.AvatarUrl ?? string.Empty,
                    Role = memberRole
                });
            }

            dtos.Add(new GetConversationsDto
            {
                Id = x.Id,
                IsGroup = x.IsGroup,
                Title = x.ConversationMembers.First(cm => cm.UserId == userId).CustomTitle,
                UpdatedAt = x.UpdatedAt,
                LastMessageContent = x.LastMessageContent,
                LastMessageType = x.LastMessageType.ToString(),
                LastMessageMediaType = x.LastMessageMediaType.ToString(),
                LastMessageSenderName = x.LastMessageSenderName,
                LastMessageSenderId = x.LastMessageSenderId,
                IsRead = !x.Messages.Any(m => m.Id == x.LastMessageId)
                    || x.Messages.FirstOrDefault(m => m.Id == x.LastMessageId)?
                    .MessageStatuses.FirstOrDefault(ms => ms.UserId == userId)?.ReadAt != null,
                ConversationImg = x.ConversationMembers.First(cm => cm.UserId == userId).ConversationImage,
                Members = members
            });
        }

        var orderedDtos = dtos.OrderByDescending(x => x.UpdatedAt).ToList();

        return new PagingResultDto<GetConversationsDto>(totalCount, orderedDtos);
    }
}