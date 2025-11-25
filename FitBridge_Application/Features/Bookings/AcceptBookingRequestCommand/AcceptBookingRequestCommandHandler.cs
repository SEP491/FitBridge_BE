using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Application.Specifications.Bookings;
using FitBridge_Domain.Exceptions;
using AutoMapper;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtBookingForValidate;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Services;
using System.Formats.Asn1;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Specifications.Messaging.GetMessageByBookingRequest;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Application.Specifications.Messaging.GetConversationMembers;
using FitBridge_Application.Dtos.Messaging;

namespace FitBridge_Application.Features.Bookings.AcceptBookingRequestCommand;

public class AcceptBookingRequestCommandHandler(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IScheduleJobServices _scheduleJobServices,
    IUserUtil userUtil,
    IMessagingHubService messagingHubService,
    IHttpContextAccessor httpContextAccessor,

    BookingService _bookingService) : IRequestHandler<AcceptBookingRequestCommand, Guid>
{
    public async Task<Guid> Handle(AcceptBookingRequestCommand request, CancellationToken cancellationToken)
    {
        var bookingRequest = await _unitOfWork.Repository<BookingRequest>().GetByIdAsync(request.BookingRequestId);
        if (bookingRequest == null)
        {
            throw new NotFoundException("Booking request not found");
        }
        if (bookingRequest.RequestStatus != BookingRequestStatus.Pending)
        {
            throw new BusinessException("Booking request is not pending");
        }
        if (bookingRequest.RequestType != RequestType.CustomerCreate && bookingRequest.RequestType != RequestType.PtCreate)
        {
            throw new BusinessException("Booking request is not customer create or pt create");
        }
        await _bookingService.ValidateBookingRequest(bookingRequest);
        var newBooking = _mapper.Map<Booking>(bookingRequest);
        bookingRequest.RequestStatus = BookingRequestStatus.Approved;
        _unitOfWork.Repository<Booking>().Insert(newBooking);
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(bookingRequest.CustomerPurchasedId);
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        customerPurchased.AvailableSessions--;
        _unitOfWork.Repository<CustomerPurchased>().Update(customerPurchased);
        _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);
        await _scheduleJobServices.ScheduleAutoCancelBookingJob(newBooking);
        await _scheduleJobServices.CancelScheduleJob($"AutoRejectBookingRequest_{bookingRequest.Id}", "AutoRejectBookingRequest");

        var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));
        var userName = userUtil.GetUserFullName(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException("User name not found");
        var message = await GetMessageAsync(request.BookingRequestId);
        await _unitOfWork.CommitAsync();
        if (message != null)
        {
            var msgContent = $"{userName} has approved the booking request";
            var newSystemMessage = await InsertSystemMessageAsync(message, msgContent);
            await SendAcceptedMessageAsync(
                message,
                msgContent,
                newSystemMessage,
                bookingRequest,
                userId);
        }
        return request.BookingRequestId;
    }

    private async Task<Message?> GetMessageAsync(Guid bookingRequestId)
    {
        var msgSpec = new GetMessageByBookingRequestSpec(bookingRequestId);
        var message = await _unitOfWork.Repository<Message>().GetBySpecificationAsync(msgSpec);
        return message;
    }

    private async Task<Message> InsertSystemMessageAsync(Message message, string msgContent)
    {
        var newSystemMessage = new Message
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
        await _unitOfWork.CommitAsync();

        return newSystemMessage;
    }

    private async Task SendAcceptedMessageAsync(
        Message message,
        string msgContent,
        Message newSystemMessage,
        BookingRequest bookingRequest,
        Guid userId)
    {
        var convo = await _unitOfWork.Repository<Conversation>().GetByIdAsync(message.ConversationId)
            ?? throw new NotFoundException(nameof(Conversation));
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
        bookingRequestDto.RequestStatus = BookingRequestStatus.Approved.ToString();
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