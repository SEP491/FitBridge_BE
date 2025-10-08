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
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(request.CustomerPurchasedId);
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        if (customerPurchased.AvailableSessions <= 0)
        {
            throw new NotEnoughSessionException("Customer purchased not enough sessions");
        }
        if(customerPurchased.AvailableSessions - request.RequestBookings.Count <= 0)
        {
            throw new NotEnoughSessionException($"Available sessions is not enough, current available sessions is: {customerPurchased.AvailableSessions}");
        }
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
                PtId = request.PtId,
                BookingName = requestBooking.BookingName,
                StartTime = requestBooking.PtFreelanceStartTime,
                EndTime = requestBooking.PtFreelanceEndTime,
                BookingDate = requestBooking.BookingDate,
                RequestStatus = BookingRequestStatus.Pending,
                RequestType = requestType,
            };
            response.Add(_mapper.Map<CreateRequestBookingResponseDto>(insertRequestBooking));
            _unitOfWork.Repository<BookingRequest>().Insert(insertRequestBooking);
        }

        await _unitOfWork.CommitAsync();
        return response;
    }
}
