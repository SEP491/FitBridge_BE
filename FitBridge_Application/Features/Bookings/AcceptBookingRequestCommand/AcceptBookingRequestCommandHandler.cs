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

namespace FitBridge_Application.Features.Bookings.AcceptBookingRequestCommand;

public class AcceptBookingRequestCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IScheduleJobServices _scheduleJobServices, BookingService _bookingService) : IRequestHandler<AcceptBookingRequestCommand, Guid>
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
        await _unitOfWork.CommitAsync();
        return request.BookingRequestId;
    }
}