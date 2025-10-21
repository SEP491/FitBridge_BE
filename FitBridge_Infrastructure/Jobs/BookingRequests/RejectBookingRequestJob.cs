using System;
using Microsoft.Extensions.Logging;
using FitBridge_Application.Interfaces.Repositories;
using Quartz;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Infrastructure.Jobs.BookingRequests;

public class RejectBookingRequestJob(ILogger<RejectBookingRequestJob> _logger, IUnitOfWork _unitOfWork) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var bookingRequestId = Guid.Parse(context.JobDetail.JobDataMap.GetString("bookingRequestId")
            ?? throw new NotFoundException($"{nameof(RejectBookingRequestJob)}_bookingRequestId"));
        _logger.LogInformation("RejectBookingRequestJob started for BookingRequest: {BookingRequestId}", bookingRequestId);
        var bookingRequest = await _unitOfWork.Repository<BookingRequest>().GetByIdAsync(bookingRequestId);
        if (bookingRequest == null)
        {
            _logger.LogError("BookingRequest not found for BookingRequestId: {BookingRequestId}", bookingRequestId);
            return;
        }
        if (bookingRequest.RequestStatus != BookingRequestStatus.Pending)
        {
            _logger.LogWarning("BookingRequest is not pending, current status: {BookingRequestStatus}", bookingRequest.RequestStatus);
            return;
        }
        bookingRequest.RequestStatus = BookingRequestStatus.Rejected;
        bookingRequest.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);
        await _unitOfWork.CommitAsync();
    }
}
