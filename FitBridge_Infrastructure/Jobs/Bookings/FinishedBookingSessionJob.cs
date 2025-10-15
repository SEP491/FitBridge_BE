using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FitBridge_Infrastructure.Jobs.Bookings;

public class FinishedBookingSessionJob(ILogger<FinishedBookingSessionJob> _logger, IUnitOfWork _unitOfWork) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var bookingId = Guid.Parse(context.JobDetail.JobDataMap.GetString("bookingId")
            ?? throw new NotFoundException($"{nameof(FinishedBookingSessionJob)}_bookingId"));
        _logger.LogInformation("FinishedBookingSessionJob started for Booking: {BookingId}", bookingId);
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(bookingId);
        if (booking == null)
        {
            _logger.LogError("Booking not found for BookingId: {BookingId}", bookingId);
            throw new NotFoundException($"{nameof(FinishedBookingSessionJob)}_bookingId");
        }
        booking.SessionStatus = SessionStatus.Finished;
        booking.SessionEndTime = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Booking>().Update(booking);
        await _unitOfWork.CommitAsync();
    }
}
