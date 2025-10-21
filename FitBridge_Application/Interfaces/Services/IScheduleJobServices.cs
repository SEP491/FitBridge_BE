using System;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Interfaces.Services;

public interface IScheduleJobServices
{
    Task<bool> ScheduleProfitDistributionJob(ProfitJobScheduleDto profitJobScheduleDto);

    Task<bool> ScheduleFinishedBookingSession(FinishedBookingSessionJobScheduleDto finishedBookingSessionJobScheduleDto);
    Task<bool> CancelScheduleJob(string jobName, string jobGroup);
    Task<bool> ScheduleAutoCancelBookingJob(Booking booking);
    Task<bool> ScheduleAutoRejectBookingRequestJob(BookingRequest bookingRequest);
}
