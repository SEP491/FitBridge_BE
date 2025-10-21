using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.Trainings;
using MediatR;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Dtos.Jobs;

namespace FitBridge_Application.Features.Bookings.StartBookingSession;

public class StartBookingSessionCommandHandler(IUnitOfWork _unitOfWork, IScheduleJobServices _scheduleJobServices) : IRequestHandler<StartBookingSessionCommand, DateTime>
{
    public async Task<DateTime> Handle(StartBookingSessionCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId, includes: new List<string> { nameof(Booking.CustomerPurchased), "CustomerPurchased.OrderItems", "CustomerPurchased.OrderItems.FreelancePTPackage" });
        if (booking == null)
        {
            throw new NotFoundException("Booking not found");
        }
        if (booking.SessionStartTime != null)
        {
            throw new BusinessException("Booking session already started");
        }
        booking.SessionStartTime = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Booking>().Update(booking);
        await ScheduleFinishedBookingSession(booking);
        await CancelAutoCancelBookingJob(booking);
        await _unitOfWork.CommitAsync();
        return booking.SessionStartTime.Value;
    }

    public async Task ScheduleFinishedBookingSession(Booking booking)
    {
        var durationInMinutes = booking.CustomerPurchased.OrderItems.OrderByDescending(x => x.CreatedAt).FirstOrDefault(x => x.FreelancePTPackageId != null)?.FreelancePTPackage?.SessionDurationInMinutes ?? 0;
        await _scheduleJobServices.ScheduleFinishedBookingSession(new FinishedBookingSessionJobScheduleDto
        {
            BookingId = booking.Id,
            TriggerTime = booking.SessionStartTime.Value.AddMinutes(durationInMinutes)
        });
    }

    public async Task CancelAutoCancelBookingJob(Booking booking)
    {
        await _scheduleJobServices.CancelScheduleJob($"AutoCancelBooking_{booking.Id}", "AutoCancelBooking");
    }
}
