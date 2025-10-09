using System;
using MediatR;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Application.Specifications.Bookings;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerIdAndPtId;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtBookingForValidation;
using FitBridge_Application.Dtos.Bookings;
using AutoMapper;
using FitBridge_Application.Commons.Constants;

namespace FitBridge_Application.Features.Bookings.CreateRequestBooking;

public class CreateRequestBookingCommandHandler(IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<CreateRequestBookingCommand, List<CreateRequestBookingResponseDto>>
{
    public async Task<List<CreateRequestBookingResponseDto>> Handle(CreateRequestBookingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var role = _userUtil.GetUserRole(_httpContextAccessor.HttpContext);
        // var bookingSpec = new GetBookingForValidationSpec(userId.Value, request.BookingDate, request.PtFreelanceStartTime, request.PtFreelanceEndTime);
        // var booking = await _unitOfWork.Repository<Booking>().GetBySpecificationAsync(bookingSpec, false);
        // if (booking != null)
        // {
        //     throw new DuplicateException($"Customer have course at this time, booking that overlapped: {booking.Id}");
        // }
        // var freelancePtBookingSpec = new GetFreelancePtBookingForValidationSpec(userId.Value, request.BookingDate, request.PtFreelanceStartTime, request.PtFreelanceEndTime);
        // var freelancePtBooking = await _unitOfWork.Repository<Booking>().GetBySpecificationAsync(freelancePtBookingSpec, false);
        // if (freelancePtBooking != null)
        // {
        //     throw new DuplicateException($"PT have course at this time booking that overlapped: {freelancePtBooking.Id}");
        // }
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(request.CustomerPurchasedId, true, new List<string> { "OrderItems.FreelancePTPackage" });

        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        if (customerPurchased.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new NotFoundException("Customer purchased expired at: " + customerPurchased.ExpirationDate);
        }
        var ptId = customerPurchased.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.PtId;
        if (customerPurchased.AvailableSessions <= 0)
        {
            throw new NotEnoughSessionException("Customer purchased not enough sessions");
        }
        if (customerPurchased.AvailableSessions - request.RequestBookings.Count <= 0)
        {
            throw new NotEnoughSessionException($"Available sessions is not enough, current available sessions is: {customerPurchased.AvailableSessions}");
        }
        await ValidateBookingRequest(request.RequestBookings);
        var requestType = RequestType.CustomerCreate;
        if (role.Equals(ProjectConstant.UserRoles.FreelancePT))
        {
            requestType = RequestType.PtCreate;
        }
        var response = new List<CreateRequestBookingResponseDto>();
        foreach (var requestBooking in request.RequestBookings)
        {
            var insertRequestBooking = new BookingRequest
            {
                CustomerId = customerPurchased.CustomerId,
                CustomerPurchasedId = customerPurchased.Id,
                PtId = ptId,
                BookingName = requestBooking.BookingName,
                StartTime = requestBooking.PtFreelanceStartTime,
                EndTime = requestBooking.PtFreelanceEndTime,
                BookingDate = requestBooking.BookingDate,
                RequestStatus = BookingRequestStatus.Pending,
                RequestType = requestType,
            };
            _unitOfWork.Repository<BookingRequest>().Insert(insertRequestBooking);
            response.Add(_mapper.Map<CreateRequestBookingResponseDto>(insertRequestBooking));

        }

        await _unitOfWork.CommitAsync();
        return response;
    }

    public async Task<bool> ValidateBookingRequest(List<CreateRequestBookingDto> requestBookings)
    {
        // Validate date and basic rules
        foreach (var requestBooking in requestBookings)
        {
            if (requestBooking.BookingDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new BusinessException($"Booking date {requestBooking.BookingDate} must be in the future");
            }

            if (requestBooking.PtFreelanceStartTime >= requestBooking.PtFreelanceEndTime)
            {
                throw new BusinessException($"End time must be after start time");
            }
        }

        // Check for overlaps within the same request (in-memory - very fast)
        for (int i = 0; i < requestBookings.Count; i++)
        {
            for (int j = i + 1; j < requestBookings.Count; j++)
            {
                var booking1 = requestBookings[i];
                var booking2 = requestBookings[j];

                // Only check if they're on the same date
                if (booking1.BookingDate == booking2.BookingDate)
                {
                    if (IsTimeSlotOverlapping(
                        booking1.PtFreelanceStartTime, booking1.PtFreelanceEndTime,
                        booking2.PtFreelanceStartTime, booking2.PtFreelanceEndTime))
                    {
                        throw new BusinessException(
                            $"Time slots overlap on {booking1.BookingDate}: " +
                            $"Slot 1: {booking1.PtFreelanceStartTime}-{booking1.PtFreelanceEndTime}, " +
                            $"Slot 2: {booking2.PtFreelanceStartTime}-{booking2.PtFreelanceEndTime}");
                    }
                }
            }
        }

        return true;
    }

    private bool IsTimeSlotOverlapping(TimeOnly start1, TimeOnly end1, TimeOnly start2, TimeOnly end2)
    {
        // Two time slots overlap if:
        // - Slot1 starts before Slot2 ends AND
        // - Slot2 starts before Slot1 ends
        return (start1 < end2 && start2 < end1) || (start1 == start2 && end1 == end2);
    }
}
