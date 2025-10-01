using System;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Commons.Constants;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Features.Bookings.CancelGymPtBooking;

public class CancelGymPtBookingCommandHandler(IUnitOfWork _unitOfWork) : IRequestHandler<CancelGymPtBookingCommand, bool>
{
    public async Task<bool> Handle(CancelGymPtBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId, false);
        if (booking == null)
        {
            throw new NotFoundException("Booking not found");
        }
        if(TimeOnly.FromDateTime(DateTime.Now) - booking.PTGymSlot.GymSlot.StartTime <= TimeSpan.FromHours(ProjectConstant.CancelBookingBeforeHours))
        {
            booking.CustomerPurchased.AvailableSessions++;
        }
        _unitOfWork.Repository<Booking>().Delete(booking);
        await _unitOfWork.CommitAsync();
        return true;
    }

}
