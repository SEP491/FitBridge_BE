using System;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Exceptions;
using AutoMapper;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Application.Specifications.Bookings;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtBookingForValidate;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Application.Specifications.Messaging.GetMessageByBookingRequest;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Application.Specifications.Messaging.GetConversationMembers;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Interfaces.Services.Messaging;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Features.Bookings.AcceptEditBookingRequest;

public class AcceptEditBookingRequestCommandHandler(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IMessagingHubService messagingHubService,
    IUserUtil userUtil,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<AcceptEditBookingRequestCommand, UpdateBookingResponseDto>
{
    public async Task<UpdateBookingResponseDto> Handle(AcceptEditBookingRequestCommand request, CancellationToken cancellationToken)
    {
        var bookingRequest = await _unitOfWork.Repository<BookingRequest>().GetByIdAsync(request.BookingRequestId);
        if (bookingRequest == null)
        {
            throw new NotFoundException("Booking request not found");
        }
        if (bookingRequest.RequestType != RequestType.PtUpdate
        && bookingRequest.RequestType != RequestType.CustomerUpdate)
        {
            throw new BusinessException("Booking request is not pt update or customer update");
        }
        if (bookingRequest.RequestStatus != BookingRequestStatus.Pending)
        {
            throw new BusinessException("Booking request is not pending");
        }
        if (bookingRequest.TargetBookingId == null)
        {
            throw new NotFoundException("Target booking id not found");
        }
        await ValidateBookingRequest(bookingRequest, bookingRequest.CustomerId, bookingRequest.PtId);
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(bookingRequest.TargetBookingId.Value);
        if (booking == null)
        {
            throw new NotFoundException("Booking not found");
        }
        booking.BookingDate = bookingRequest.BookingDate;
        booking.PtFreelanceStartTime = bookingRequest.StartTime;
        booking.PtFreelanceEndTime = bookingRequest.EndTime;
        booking.BookingName = bookingRequest.BookingName;
        booking.Note = bookingRequest.Note;
        booking.SessionStatus = SessionStatus.Booked;
        bookingRequest.RequestStatus = BookingRequestStatus.Approved;
        _unitOfWork.Repository<Booking>().Update(booking);
        _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);

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
        return _mapper.Map<UpdateBookingResponseDto>(booking);
    }

    public async Task<bool> ValidateBookingRequest(BookingRequest request, Guid customerId, Guid ptId)
    {
        var bookingSpec = new GetBookingForValidationSpec(customerId, request.BookingDate, request.StartTime, request.EndTime);
        var booking = await _unitOfWork.Repository<Booking>().GetAllWithSpecificationAsync(bookingSpec);
        if (booking.Count > 0)
        {
            if (booking.Count == 1 && booking.First().Id == request.TargetBookingId)
            {
            }
            else
            {
                throw new DuplicateException($"Customer have course at this time, booking that overlapped: {booking.First().Id}");
            }
        }
        var freelancePtBookingSpec = new GetFreelancePtBookingForValidationSpec(ptId, request.BookingDate, request.StartTime, request.EndTime);
        var freelancePtBooking = await _unitOfWork.Repository<Booking>().GetAllWithSpecificationAsync(freelancePtBookingSpec);
        if (freelancePtBooking.Count > 0)
        {
            if (freelancePtBooking.Count == 1 && freelancePtBooking.First().Id == request.TargetBookingId)
            {
            }
            else
            {
                throw new DuplicateException($"Customer have course at this time, booking that overlapped: {freelancePtBooking.First().Id}");
            }
        }
        return true;
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