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
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId, false
        , includes: new List<string> { "PTGymSlot", "PTGymSlot.GymSlot", "CustomerPurchased" }
        );
        if (booking == null)
        {
            throw new NotFoundException("Booking not found");
        }
        var sessionDateTime = booking.BookingDate.ToDateTime(booking.PTGymSlot.GymSlot.StartTime);
        if (sessionDateTime < DateTime.UtcNow)
        {
            throw new BusinessException("Cannot cancel a session that has already occurred");
        }

        var hoursUntilSession = sessionDateTime - DateTime.UtcNow;

        // Check cancellation policy and refund session if applicable
        if (hoursUntilSession.TotalHours > ProjectConstant.CancelBookingBeforeHours)
        {
            if (booking.CustomerPurchased == null)
            {
                throw new InvalidOperationException("CustomerPurchased not found for booking");
            }
            booking.CustomerPurchased.AvailableSessions++;
        }
        _unitOfWork.Repository<Booking>().Delete(booking);
        await _unitOfWork.CommitAsync();
        return true;
    }

}
