using System;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Domain.Entities.Trainings;
using Quartz;

namespace FitBridge_Application.Interfaces.Services;

public interface IScheduleJobServices
{
    Task<bool> ScheduleProfitDistributionJob(ProfitJobScheduleDto profitJobScheduleDto);

    Task<bool> ScheduleFinishedBookingSession(FinishedBookingSessionJobScheduleDto finishedBookingSessionJobScheduleDto);
    Task<bool> CancelScheduleJob(string jobName, string jobGroup);
    Task<bool> ScheduleAutoCancelBookingJob(Booking booking);
    Task<bool> ScheduleAutoRejectBookingRequestJob(BookingRequest bookingRequest);
    Task<TriggerState> GetJobStatus(string jobName, string jobGroup);
    Task<bool> RescheduleJob(string jobName, string jobGroup, DateTime triggerTime);
    Task<bool> ScheduleExpireUserSubscriptionJob(Guid UserSubscriptionId, DateTime triggerTime);
    Task<bool> ScheduleSendRemindExpiredSubscriptionNotiJob(Guid UserSubscriptionId, DateTime triggerTime);
    Task<bool> ScheduleAutoRejectEditBookingRequestJob(Guid BookingRequestId, DateTime triggerTime);
}
