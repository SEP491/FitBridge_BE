using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Application.Specifications.Messaging.GetConversationMembers;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Specifications.Messaging.GetMessageByBookingRequest;

namespace FitBridge_Application.Features.Bookings.RejectBookingRequest;

public class RejectBookingRequestCommandHandler(
    IUnitOfWork _unitOfWork,
    IUserUtil userUtil,
    IHttpContextAccessor httpContextAccessor,
    IMessagingHubService messagingHubService) : IRequestHandler<RejectBookingRequestCommand, bool>
{
    public async Task<bool> Handle(RejectBookingRequestCommand request, CancellationToken cancellationToken)
    {
        var bookingRequest = await _unitOfWork.Repository<BookingRequest>().GetByIdAsync(request.BookingRequestId, false, new List<string> { "TargetBooking" });
        if (bookingRequest == null)
        {
            throw new NotFoundException("Booking request not found");
        }
        if (bookingRequest.RequestStatus != BookingRequestStatus.Pending)
        {
            throw new BusinessException("Booking request is not pending, current status: " + bookingRequest.RequestStatus);
        }
        if (bookingRequest.TargetBooking != null)
        {
            bookingRequest.TargetBooking.SessionStatus = SessionStatus.Booked;
        }
        bookingRequest.RequestStatus = BookingRequestStatus.Rejected;
        bookingRequest.UpdatedAt = DateTime.UtcNow;

        var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));
        var userName = userUtil.GetUserFullName(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException("User name not found");
        var message = await GetMessageAsync(request.BookingRequestId);
        if (message != null)
        {
            var msgContent = $"{userName} has rejected the booking request";
            InsertSystemMessage(message, msgContent, out var newSystemMessage);
            await _unitOfWork.CommitAsync();
            await SendRejectedMessage(
                message,
                msgContent,
                newSystemMessage,
                bookingRequest,
                userId);
        }
        return true;
    }

    private async Task<Message?> GetMessageAsync(Guid bookingRequestId)
    {
        var msgSpec = new GetMessageByBookingRequestSpec(bookingRequestId);
        var message = await _unitOfWork.Repository<Message>().GetBySpecificationAsync(msgSpec);
        return message;
    }

    private void InsertSystemMessage(Message message, string msgContent, out Message newSystemMessage)
    {
        newSystemMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = msgContent,
            ConversationId = message.ConversationId,
            MediaType = MediaType.Text,
            MessageType = MessageType.System,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
        };

        _unitOfWork.Repository<Message>().Insert(newSystemMessage);
    }

    private async Task SendRejectedMessage(
        Message message,
        string msgContent,
        Message newSystemMessage,
        BookingRequest bookingRequest,
        Guid userId)
    {
        var convo = await _unitOfWork.Repository<Conversation>().GetByIdAsync(message.ConversationId);
        convo.LastMessageMediaType = MediaType.BookingRequest;
        convo.LastMessageContent = msgContent;
        convo.LastMessageId = newSystemMessage.Id;
        convo.LastMessageType = MessageType.System;
        convo.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Conversation>().Update(convo);

        await _unitOfWork.CommitAsync();
        var convoMemberSpec = new GetConversationMembersSpec(message.ConversationId);
        var users = (await _unitOfWork.Repository<ConversationMember>()
            .GetAllWithSpecificationAsync(convoMemberSpec))
            .Select(x => x.UserId.ToString());

        var dtoSystemMessage = new MessageReceivedDto
        {
            Id = newSystemMessage.Id,
            ConversationId = message.ConversationId,
            MessageType = MessageType.System.ToString(),
            Content = newSystemMessage.Content,
            CreatedAt = DateTime.UtcNow,
            MediaType = MediaType.Text.ToString(),
        };

        await messagingHubService.NotifyUsers(dtoSystemMessage, users);
        var bookingRequestDto = BookingRequestDto.FromEntity(bookingRequest);
        bookingRequestDto.RequestStatus = BookingRequestStatus.Rejected.ToString();
        var dtoUpdatedMessageSchedule = new MessageUpdatedDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            MessageType = MessageType.System.ToString(),
            BookingRequest = bookingRequestDto,
            Status = "Updated"
        };

        await messagingHubService.NotifyUsers(dtoUpdatedMessageSchedule, users.Except([userId.ToString()]));
    }
}