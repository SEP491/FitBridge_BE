using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.Bookings.RejectBookingRequest;

public class RejectBookingRequestCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<RejectBookingRequestCommand, bool>
{
    public async Task<bool> Handle(RejectBookingRequestCommand request, CancellationToken cancellationToken)
    {
        var bookingRequest = await _unitOfWork.Repository<BookingRequest>().GetByIdAsync(request.BookingRequestId, false, new List<string> { "TargetBooking" });
        if (bookingRequest == null)
        {
            throw new NotFoundException("Booking request not found");
        }
        if(bookingRequest.RequestStatus != BookingRequestStatus.Pending)
        {
            throw new BusinessException("Booking request is not pending, current status: " + bookingRequest.RequestStatus);
        }
        if(bookingRequest.TargetBooking != null)
        {
            bookingRequest.TargetBooking.SessionStatus = SessionStatus.Booked;
        }
        bookingRequest.RequestStatus = BookingRequestStatus.Rejected;
        bookingRequest.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.CommitAsync();
        return true;
    }
}
