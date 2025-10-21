using System;
using Quartz;
using Microsoft.Extensions.Logging;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Infrastructure.Jobs.Bookings;

public class CancelBookingJob(ILogger<CancelBookingJob> _logger, IUnitOfWork _unitOfWork) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var bookingId = Guid.Parse(context.JobDetail.JobDataMap.GetString("bookingId")
            ?? throw new NotFoundException($"{nameof(CancelBookingJob)}_bookingId"));
        _logger.LogInformation("CancelBookingJob started for Booking: {BookingId}", bookingId);
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(bookingId);
        if (booking == null)
        {
            _logger.LogError("Booking not found for BookingId: {BookingId}", bookingId);
            return;
        }
        if (booking.SessionStatus != SessionStatus.Booked)
        {
            _logger.LogWarning("Booking is not booked, current status: {SessionStatus}", booking.SessionStatus);
            return;
        }
        booking.SessionStatus = SessionStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Booking>().Update(booking);
        await _unitOfWork.CommitAsync();
    }
}
