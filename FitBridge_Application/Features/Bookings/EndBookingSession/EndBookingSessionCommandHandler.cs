using System;
using MediatR;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Dtos.Jobs;
using Microsoft.Extensions.Logging;
namespace FitBridge_Application.Features.Bookings.EndBookingSession;

public class EndBookingSessionCommandHandler(IUnitOfWork _unitOfWork, IScheduleJobServices _scheduleJobServices, ITransactionService _transactionService,
ILogger<EndBookingSessionCommandHandler> _logger) : IRequestHandler<EndBookingSessionCommand, DateTime>
{
    public async Task<DateTime> Handle(EndBookingSessionCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId);
        if (booking == null)
        {
            throw new NotFoundException("Booking not found");
        }
        if (booking.SessionStartTime == null)
        {
            throw new BusinessException("Booking session not started");
        }
        if (booking.SessionEndTime != null)
        {
            throw new BusinessException("Booking session already ended");
        }
        booking.SessionEndTime = DateTime.UtcNow;
        booking.SessionStatus = SessionStatus.Finished;
        booking.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Booking>().Update(booking);
        await _unitOfWork.CommitAsync();
        await _scheduleJobServices.CancelScheduleJob($"FinishedBookingSession_{request.BookingId}", "FinishedBookingSession");
        var distributePendingProfitResult = await _transactionService.DistributePendingProfit(booking.CustomerPurchasedId);
        if (!distributePendingProfitResult)
        {
            _logger.LogError($"Failed to distribute pending profit for customer purchased {booking.CustomerPurchasedId}");
        }
        return booking.SessionEndTime.Value;
    }

}
