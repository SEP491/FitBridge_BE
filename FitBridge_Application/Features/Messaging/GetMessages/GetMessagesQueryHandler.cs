using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications.Messaging.GetMessages;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using CurrentMessageStatus = FitBridge_Domain.Enums.MessageAndReview.CurrentMessageStatus;

namespace FitBridge_Application.Features.Messaging.GetMessages
{
    internal class GetMessagesQueryHandler(
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        MessagingService messagingService) : IRequestHandler<GetMessagesQuery, IEnumerable<GetMessagesDto>>
    {
        public async Task<IEnumerable<GetMessagesDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext);
            var spec = new GetMessagesSpec(request.ConversationId,
                userId: userId,
                parameters: request.Params,
                includeOwnMessageStatus: true,
                includeBookingRequest: true,
                includeReplyToMessage: true,
                includeSender: true,
                includeConversationMembers: true);
            var messages = await unitOfWork.Repository<Message>().GetAllWithSpecificationAsync(spec);

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
                SenderId = x.Sender.UserId,
                Reaction = x.Reaction ?? string.Empty,
                SenderName = x.Sender?.User.UserName,
                SenderAvatarUrl = x.Sender?.User.AvatarUrl,
                BookingRequest = x.BookingRequest != null ? MapBookingRequestDto(x.BookingRequest) : null
            });

            return dtos;
        }

        private static BookingRequestDto MapBookingRequestDto(FitBridge_Domain.Entities.Trainings.BookingRequest bookingRequest)
        {
            return new BookingRequestDto
            {
                TargetBookingId = bookingRequest.TargetBookingId,
                CustomerPurchasedId = bookingRequest.CustomerPurchasedId,
                Note = bookingRequest.Note,
                StartTime = bookingRequest.StartTime,
                EndTime = bookingRequest.EndTime,
                BookingName = bookingRequest.BookingName,
                RequestStatus = bookingRequest.RequestStatus.ToString(),
                BookingDate = bookingRequest.BookingDate,
                RequestType = bookingRequest.RequestType.ToString()
            };
        }
    }
}