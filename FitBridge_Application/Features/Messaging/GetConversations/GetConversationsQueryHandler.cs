using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetConversations;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Messaging.GetConversations;

internal class GetConversationsQueryHandler(
    IUnitOfWork unitOfWork,
    IUserUtil userUtil,
    Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetConversationsQuery, PagingResultDto<GetConversationsDto>>
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

        var dtos = conversations.Select(x => new GetConversationsDto
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
            Members = x.ConversationMembers.Select(cm => new GetConversationMembersDto
            {
                UserId = cm.UserId,
                Username = cm.User.UserName ?? string.Empty,
                ImgUrl = cm.User.AvatarUrl ?? string.Empty,
                Role = userRole
            })
        }).OrderByDescending(x => x.UpdatedAt).ToList();

        return new PagingResultDto<GetConversationsDto>(totalCount, dtos);
    }
}