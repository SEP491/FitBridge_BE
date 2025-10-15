using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.Trainings;
using MediatR;

namespace FitBridge_Application.Features.Bookings.StartBookingSession;

public class StartBookingSessionCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<StartBookingSessionCommand, DateTime>
{
    public async Task<DateTime> Handle(StartBookingSessionCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId);
        if (booking == null)
        {
            throw new NotFoundException("Booking not found");
        }
        if(booking.SessionStartTime != null)
        {
            throw new BusinessException("Booking session already started");
        }
        booking.SessionStartTime = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Booking>().Update(booking);
        await _unitOfWork.CommitAsync();
        return booking.SessionStartTime.Value;
    }
}
