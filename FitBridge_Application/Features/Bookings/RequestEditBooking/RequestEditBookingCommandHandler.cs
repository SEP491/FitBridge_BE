using System;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Features.Bookings.RequestEditBooking;
using FitBridge_Application.Specifications.Bookings;
using AutoMapper;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtBookingForValidate;

namespace FitBridge_Application.Features.Bookings.RequestEditBooking;

public class RequestEditBookingCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<RequestEditBookingCommand, EditBookingResponseDto>
{
    public async Task<EditBookingResponseDto> Handle(RequestEditBookingCommand request, CancellationToken cancellationToken)
    {
        var requestType = RequestType.CustomerUpdate;
        var userRoles = _userUtil.GetUserRole(_httpContextAccessor.HttpContext);
        if (userRoles == null)
        {
            throw new NotFoundException("User role not found");
        }
        if (userRoles.Equals(ProjectConstant.UserRoles.FreelancePT))
        {
            requestType = RequestType.PtUpdate;
        }
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.TargetBookingId, true, new List<string> { "CustomerPurchased.OrderItems.FreelancePTPackage" });
        if (booking == null)
        {
            throw new NotFoundException("Booking not found");
        }
        if (booking.PtId == null)
        {
            throw new NotFoundException("PTId not found");
        }
        var maximumPracticeTime = booking.CustomerPurchased.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.SessionDurationInMinutes;

        await ValidateBookingRequest(request, booking.CustomerId, booking.PtId.Value, maximumPracticeTime);
        var editBookingRequest = new BookingRequest
        {
            CustomerId = booking.CustomerId,
            PtId = booking.PtId.Value,
            CustomerPurchasedId = booking.CustomerPurchasedId,
            BookingDate = request.BookingDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            BookingName = request.BookingName,
            TargetBookingId = request.TargetBookingId,
            Note = request.Note,
            RequestType = requestType,
            RequestStatus = BookingRequestStatus.Pending,
        };
        _unitOfWork.Repository<BookingRequest>().Insert(editBookingRequest);
        booking.SessionStatus = SessionStatus.WaitingForEdit;
        _unitOfWork.Repository<Booking>().Update(booking);
        var bookingRequestResponse = _mapper.Map<EditBookingResponseDto>(editBookingRequest);
        bookingRequestResponse.OriginalStartTime = booking.PtFreelanceStartTime.Value;
        bookingRequestResponse.OriginalEndTime = booking.PtFreelanceEndTime.Value;
        await _unitOfWork.CommitAsync();
        return bookingRequestResponse;
    }
    
    public async Task<bool> ValidateBookingRequest(RequestEditBookingCommand request, Guid customerId, Guid ptId, int maximumPracticeTime)
    {
        if(request.EndTime - request.StartTime > TimeSpan.FromMinutes(maximumPracticeTime))
        {
            throw new BusinessException($"Practice time must be less than {maximumPracticeTime} minutes");
        }
        var bookingSpec = new GetBookingForValidationSpec(customerId, request.BookingDate, request.StartTime, request.EndTime);
        var booking = await _unitOfWork.Repository<Booking>().GetAllWithSpecificationAsync(bookingSpec);
        if (booking.Count > 0 )
        {
            if(booking.Count == 1 && booking.First().Id == request.TargetBookingId)
            {
            } else {
                throw new DuplicateException($"Customer have course at this time, booking that overlapped: {booking.First().Id}");
            }
        }
        var freelancePtBookingSpec = new GetFreelancePtBookingForValidationSpec(ptId, request.BookingDate, request.StartTime, request.EndTime);
        var freelancePtBooking = await _unitOfWork.Repository<Booking>().GetAllWithSpecificationAsync(freelancePtBookingSpec);
        if (freelancePtBooking.Count > 0 )
        {
            if(freelancePtBooking.Count == 1 && freelancePtBooking.First().Id == request.TargetBookingId)
            {
            } else {
                throw new DuplicateException($"Customer have course at this time, booking that overlapped: {freelancePtBooking.First().Id}");
            }
        }
        return true;
    }
}
