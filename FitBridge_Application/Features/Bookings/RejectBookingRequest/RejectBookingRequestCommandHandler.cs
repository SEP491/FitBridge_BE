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
        var bookingRequest = await _unitOfWork.Repository<BookingRequest>().GetByIdAsync(request.BookingRequestId);
        if (bookingRequest == null)
        {
            throw new NotFoundException("Booking request not found");
        }
        if(bookingRequest.RequestStatus != BookingRequestStatus.Pending)
        {
            throw new BusinessException("Booking request is not pending, current status: " + bookingRequest.RequestStatus);
        }
        bookingRequest.RequestStatus = BookingRequestStatus.Rejected;
        bookingRequest.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
