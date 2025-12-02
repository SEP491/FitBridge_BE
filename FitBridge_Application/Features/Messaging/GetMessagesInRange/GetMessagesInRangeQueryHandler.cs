using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications;
using FitBridge_Application.Specifications.Messaging.GetMessages;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.GetMessagesInRange
{
    internal class GetMessagesInRangeQueryHandler(
        MessagingService messagingService,
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetMessagesInRangeQuery, IEnumerable<GetMessagesDto>>
    {
        public async Task<IEnumerable<GetMessagesDto>> Handle(GetMessagesInRangeQuery request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            if (request.CurrentPage <= 0) request.CurrentPage = 1;

            var targetMessageIndex = await messagingService.GetMessageIndexAsync(request.ConversationId, request.TargetMessageId);
            var targetPageNumber = (targetMessageIndex / BaseParams.DefaultPageSize) + 1;

            if (targetPageNumber <= request.CurrentPage) return [];

            var spec = new GetMessagesSpec(
                request.ConversationId,
                userId,
                skip: BaseParams.DefaultPageSize * request.CurrentPage,
                take: BaseParams.DefaultPageSize * (targetPageNumber - request.CurrentPage),
                includeBookingRequest: true,
                includeConversationMembers: true,
                includeOwnMessageStatus: true,
                includeReplyToMessage: true,
                includeSender: true);

            var messages = await unitOfWork
                .Repository<Message>()
                .GetAllWithSpecificationAsync(spec);

            var dtos = messages.Select(x => new GetMessagesDto
            {
                Id = x.Id,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                IsDeleted = x.DeletedAt != null,
                MediaType = x.MediaType.ToString(),
                MessageType = x.MessageType.ToString(),
                ConversationId = x.ConversationId,
                DeliveryStatus = x.MessageStatuses.FirstOrDefault() == null
                    ? CurrentMessageStatus.None.ToString()
                    : x.MessageStatuses.First().CurrentStatus.ToString(),
                Status = messagingService.GetMessageStatus(x),
                ReplyToMessageId = x.ReplyToMessageId,
                ReplyToMessageContent = x.ReplyToMessage?.Content,
                ReplyToMessageMediaType = x.ReplyToMessage?.MediaType.ToString(),
                SenderId = x.Sender?.UserId,
                Reaction = x.Reaction ?? string.Empty,
                SenderName = x.Sender?.User.FullName,
                SenderAvatarUrl = x.MessageType != MessageType.System
                    ? x.Sender?.User.AvatarUrl
                    : null,
                BookingRequest = x.BookingRequest != null ? BookingRequestDto.FromEntity(x.BookingRequest) : null
            });

            return dtos;
        }
    }
}