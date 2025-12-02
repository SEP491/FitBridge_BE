using AutoMapper;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Features.Bookings.CreateBooking;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Messaging.GetConversationMembers;
using FitBridge_Application.Specifications.Messaging.GetOtherConversationMembers;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Messaging.SendMessage
{
    internal class SendMessageCommandHandler(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IMessagingHubService messagingHubService) : IRequestHandler<SendMessageCommand>
    {
        public async Task Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var senderId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(ApplicationUser));
            var senderName = userUtil.GetUserFullName(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(ApplicationUser));
            var senderAvatar = userUtil.GetAvatarUrl(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException("User avatar not found");

            var convo = await unitOfWork.Repository<Conversation>().GetByIdAsync(request.ConversationId);
            if (convo == null)
            {
                throw new NotFoundException(nameof(Conversation));
            }

            var spec = new GetConversationMembersSpec(request.ConversationId);
            var conversationMembers = await unitOfWork.Repository<ConversationMember>().GetAllWithSpecificationAsync(spec);

            // Get the sender's ConversationMember record
            var senderMember = conversationMembers.FirstOrDefault(x => x.UserId == senderId)
                ?? throw new NotFoundException("Sender is not a member of this conversation");

            var newMessageGuid = Guid.NewGuid();
            var mediaType = Enum.Parse<MediaType>(request.MediaType);
            var newMessage = new Message
            {
                Id = newMessageGuid,
                Content = request.Content ?? string.Empty,
                ConversationId = request.ConversationId,
                MediaType = mediaType,
                Metadata = null,
                ReplyToMessageId = request.ReplyToMessageId ?? null,
                CreatedAt = now,
                UpdatedAt = null,
                SenderId = senderMember.Id, // Use ConversationMember.Id
            };

            List<CreateRequestBookingResponseDto?> newBookingRequest = [];
            BookingRequest? bookingRequest = null;
            Message? newSystemMessage = null;
            if (mediaType == MediaType.BookingRequest)
            {
                if (request.CreateBookingRequest == null)
                {
                    throw new DataValidationFailedException("CreateBookingRequest data must be provided for BookingRequest media type");
                }
                if (request.CustomerPurchasedId == null)
                {
                    throw new DataValidationFailedException("CustomerPurchasedId must be provided for BookingRequest media type");
                }
                newSystemMessage = new Message
                {
                    Id = Guid.NewGuid(),
                    Content = $"{senderName} has created a booking request",
                    ConversationId = request.ConversationId,
                    MediaType = MediaType.Text,
                    MessageType = MessageType.System,
                    CreatedAt = DateTime.UtcNow.AddSeconds(-5), // have the system message created b4 the schedule
                    UpdatedAt = null,
                };

                unitOfWork.Repository<Message>().Insert(newSystemMessage);

                var createBookingRequestCommand = new CreateRequestBookingCommand
                {
                    CustomerPurchasedId = request.CustomerPurchasedId.Value,
                    RequestBookings = [request.CreateBookingRequest]
                };
                newBookingRequest = await mediator.Send(createBookingRequestCommand, cancellationToken);
                newMessage.BookingRequestId = newBookingRequest[0].Id; // TODO: add support for list later
                bookingRequest = await unitOfWork.Repository<BookingRequest>()
                    .GetByIdAsync(newMessage.BookingRequestId.Value);
            }
            unitOfWork.Repository<Message>().Insert(newMessage);

            var newMessageStatus = new MessageStatus
            {
                Id = Guid.NewGuid(),
                MessageId = newMessage.Id,
                UserId = senderMember.Id, // Use ConversationMember.Id
                CurrentStatus = CurrentMessageStatus.Sent,
                ReadAt = now,
                SentAt = now,
                DeliveredAt = null
            };
            unitOfWork.Repository<MessageStatus>().Insert(newMessageStatus);

            // update convo
            if (newSystemMessage != null)
            {
                convo.LastMessageContent = $"{senderName} has created a booking request";
                convo.LastMessageMediaType = MediaType.BookingRequest;
                convo.LastMessageType = MessageType.System;
                convo.LastMessageId = newSystemMessage.Id;
                convo.UpdatedAt = now;
            }
            else
            {
                convo.LastMessageSenderName = senderName;
                convo.LastMessageSenderId = senderMember.Id; // Use ConversationMember.Id

                convo.LastMessageContent = request.Content ?? string.Empty;
                convo.LastMessageMediaType = mediaType;
                convo.LastMessageType = MessageType.User;
                convo.LastMessageId = newMessage.Id;
                convo.UpdatedAt = now;
            }
            unitOfWork.Repository<Conversation>().Update(convo);

            await unitOfWork.CommitAsync();

            // For SignalR notifications, we still use the UserId
            var userIds = conversationMembers.Select(x => x.UserId.ToString()).ToList();

            if (newSystemMessage != null)
            {
                var dtoSystemMessage = new MessageReceivedDto
                {
                    Id = newSystemMessage.Id,
                    ConversationId = request.ConversationId,
                    MessageType = MessageType.System.ToString(),
                    Content = newSystemMessage.Content,
                    CreatedAt = now,
                    MediaType = MediaType.Text.ToString(),
                };
                await messagingHubService.NotifyUsers(dtoSystemMessage, userIds.AsReadOnly());
            }
            var dto = new MessageReceivedDto
            {
                Id = newMessageGuid,
                ConversationId = request.ConversationId,
                MessageType = MessageType.User.ToString(),
                Content = request.Content ?? string.Empty,
                CreatedAt = now,
                MediaType = mediaType.ToString(),
                Metadata = null,
                SenderAvatarUrl = senderAvatar,
                SenderId = senderId,
                SenderName = senderName,
                ReplyToMessageId = request.ReplyToMessageId,
                ReplyToMessageContent = request.ReplyToMessageContent,
                ReplyToMessageMediaType = request.ReplyToMessageMediaType,
                BookingRequest = bookingRequest
                                    != null ? BookingRequestDto.FromEntity(bookingRequest) : null
            };
            await messagingHubService.NotifyUsers(dto, userIds);
        }
    }
}