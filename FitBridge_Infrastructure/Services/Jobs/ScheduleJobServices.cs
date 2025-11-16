using System;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Application.Interfaces.Services;
using Quartz;
using Microsoft.Extensions.Logging;
using FitBridge_Infrastructure.Jobs;
using FitBridge_Infrastructure.Jobs.Bookings;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Trainings;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Build.Framework;
using FitBridge_Infrastructure.Jobs.BookingRequests;
using FitBridge_Infrastructure.Jobs.Subscriptions;
using FitBridge_Application.Features.Jobs.RejectEditBookingRequest;
using FitBridge_Infrastructure.Jobs.Orders;

namespace FitBridge_Infrastructure.Services.Jobs;

public class ScheduleJobServices(ISchedulerFactory _schedulerFactory, ILogger<ScheduleJobServices> _logger) : IScheduleJobServices
{
    public async Task<bool> ScheduleProfitDistributionJob(ProfitJobScheduleDto profitJobScheduleDto)
    {
        var jobKey = new JobKey($"ProfitDistribution_{profitJobScheduleDto.OrderItemId}", "ProfitDistribution");
        var triggerKey = new TriggerKey($"ProfitDistribution_{profitJobScheduleDto.OrderItemId}_Trigger", "ProfitDistribution");
        var jobData = new JobDataMap
        {
            { "orderItemId", profitJobScheduleDto.OrderItemId.ToString() }
        };
        var job = JobBuilder.Create<DistributeProfitJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();

        var triggerTime = profitJobScheduleDto.ProfitDistributionDate.ToDateTime(TimeOnly.MinValue);
        // var triggerTime = DateTime.Now.AddSeconds(20);
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();

        await _schedulerFactory.GetScheduler().Result
        .ScheduleJob(job, trigger);

        _logger.LogInformation(
        "Scheduled profit distribution job for OrderItem {OrderItemId} at {TriggerTime}",
        profitJobScheduleDto.OrderItemId, triggerTime);
        return true;
    }

    public async Task<bool> ScheduleFinishedBookingSession(FinishedBookingSessionJobScheduleDto finishedBookingSessionJobScheduleDto)
    {
        var jobKey = new JobKey($"FinishedBookingSession_{finishedBookingSessionJobScheduleDto.BookingId}", "FinishedBookingSession");
        var triggerKey = new TriggerKey($"FinishedBookingSession_{finishedBookingSessionJobScheduleDto.BookingId}_Trigger", "FinishedBookingSession");
        var jobData = new JobDataMap
        {
            { "bookingId", finishedBookingSessionJobScheduleDto.BookingId.ToString()}
        };
        var job = JobBuilder.Create<FinishedBookingSessionJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();

        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(finishedBookingSessionJobScheduleDto.TriggerTime)
        .Build();

        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation("Scheduled finished booking session job for Booking {BookingId} at {TriggerTime}", finishedBookingSessionJobScheduleDto.BookingId, finishedBookingSessionJobScheduleDto.TriggerTime);
        return true;
    }

    public async Task<bool> CancelScheduleJob(string jobName, string jobGroup)
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey(jobName, jobGroup);
            var exists = await scheduler.CheckExists(jobKey);
            if (!exists)
            {
                _logger.LogWarning("Job {JobName} in {JobGroup} does not exist", jobName, jobGroup);
                return false;
            }
            var result = await scheduler.DeleteJob(jobKey);
            if (result)
            {
                _logger.LogInformation("Successfully cancelled job: {JobName} in group {JobGroup}",
                    jobName, jobGroup);
            }
            else
            {
                _logger.LogWarning("Failed to cancel job: {JobName} in group {JobGroup}",
                    jobName, jobGroup);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling schedule job for {JobName} in {JobGroup}", jobName, jobGroup);
            throw new BusinessException($"Failed to cancel job: {ex.Message}");
        }
    }

    public async Task<bool> ScheduleAutoCancelBookingJob(Booking booking)
    {
        var jobKey = new JobKey($"AutoCancelBooking_{booking.Id}", "AutoCancelBooking");
        var triggerKey = new TriggerKey($"AutoCancelBooking_{booking.Id}_Trigger", "AutoCancelBooking");
        var jobData = new JobDataMap
        {
            { "bookingId", booking.Id.ToString() }
        };
        var triggerTime = booking.BookingDate.ToDateTime(booking.PtFreelanceEndTime.Value);
        var job = JobBuilder.Create<CancelBookingJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);

        _logger.LogInformation($"Successfully scheduled auto cancel job for booking {booking.Id} at {triggerTime}");
        return true;
    }

    public async Task<bool> ScheduleAutoRejectBookingRequestJob(BookingRequest bookingRequest)
    {
        var jobKey = new JobKey($"AutoRejectBookingRequest_{bookingRequest.Id}", "AutoRejectBookingRequest");
        var triggerKey = new TriggerKey($"AutoRejectBookingRequest_{bookingRequest.Id}_Trigger", "AutoRejectBookingRequest");
        var jobData = new JobDataMap
        {
            { "bookingRequestId", bookingRequest.Id.ToString() }
        };
        var triggerTime = bookingRequest.BookingDate.ToDateTime(bookingRequest.StartTime);
        var job = JobBuilder.Create<RejectBookingRequestJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();

        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled auto reject booking request job for booking request {bookingRequest.Id} at {triggerTime}");
        return true;
    }

    public async Task<TriggerState> GetJobStatus(string jobName, string jobGroup)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobName, jobGroup);
        var exists = await scheduler.CheckExists(jobKey);
        if (!exists)
        {
            return TriggerState.None;
        }
        var triggers = await scheduler.GetTriggersOfJob(jobKey);
        if (triggers.Count <= 0)
        {
            throw new NotFoundException($"Job {jobName} in {jobGroup} has no triggers");
        }
        var trigger = triggers.First();
        var triggerState = await scheduler.GetTriggerState(trigger.Key);

        _logger.LogInformation($"Job {jobName} in {jobGroup} is {triggerState}");

        return triggerState;
    }

    public async Task<bool> RescheduleJob(string jobName, string jobGroup, DateTime triggerTime)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobName, jobGroup);
        var exists = await scheduler.CheckExists(jobKey);
        if (!exists)
        {
            throw new NotFoundException($"Job {jobName} in {jobGroup} does not exist");
        }
        var triggerKey = new TriggerKey($"{jobName}_Trigger", jobGroup);
        var checkTriggerExists = await scheduler.CheckExists(triggerKey);
        if (!checkTriggerExists)
        {
            throw new NotFoundException($"Trigger {triggerKey} does not exist");
        }
        var newTrigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await scheduler.RescheduleJob(triggerKey, newTrigger);
        return true;
    }

    public async Task<bool> ScheduleExpireUserSubscriptionJob(Guid UserSubscriptionId, DateTime triggerTime)
    {
        var jobKey = new JobKey($"ExpireUserSubscription_{UserSubscriptionId}", "ExpireUserSubscription");
        var triggerKey = new TriggerKey($"ExpireUserSubscription_{UserSubscriptionId}_Trigger", "ExpireUserSubscription");
        var jobData = new JobDataMap
        {
            { "userSubscriptionId", UserSubscriptionId.ToString() }
        };
        var job = JobBuilder.Create<ExpireUserSubscriptionJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled expire user subscription job for user subscription {UserSubscriptionId} at {triggerTime.ToLocalTime}");
        return true;
    }

    public async Task<bool> ScheduleSendRemindExpiredSubscriptionNotiJob(Guid UserSubscriptionId, DateTime triggerTime)
    {
        var jobKey = new JobKey($"SendRemindExpiredSubscriptionNoti_{UserSubscriptionId}", "SendRemindExpiredSubscriptionNoti");
        var triggerKey = new TriggerKey($"SendRemindExpiredSubscriptionNoti_{UserSubscriptionId}_Trigger", "SendRemindExpiredSubscriptionNoti");
        var jobData = new JobDataMap
        {
            { "userSubscriptionId", UserSubscriptionId.ToString() }
        };
        var job = JobBuilder.Create<SendRemindExpiredSubscriptionNotiJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled send remind expired subscription notification job for user subscription {UserSubscriptionId} at {triggerTime.ToLocalTime}");
        return true;
    }

    public async Task<bool> ScheduleAutoRejectEditBookingRequestJob(Guid BookingRequestId, DateTime triggerTime)
    {
        var jobKey = new JobKey($"AutoRejectEditBookingRequest_{BookingRequestId}", "AutoRejectEditBookingRequest");
        var triggerKey = new TriggerKey($"AutoRejectEditBookingRequest_{BookingRequestId}_Trigger", "AutoRejectEditBookingRequest");
        var jobData = new JobDataMap
        {
            { "bookingRequestId", BookingRequestId.ToString() }
        };
        var job = JobBuilder.Create<RejectEditBookingRequestJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled auto reject edit booking request job for booking request {BookingRequestId} at {triggerTime.ToLocalTime}");
        return true;
    }

    public async Task<bool> ScheduleAutoFinishArrivedOrderJob(Guid OrderId, DateTime triggerTime)
    {
        var jobKey = new JobKey($"AutoFinishArrivedOrder_{OrderId}", "AutoFinishArrivedOrder");
        var triggerKey = new TriggerKey($"AutoFinishArrivedOrder_{OrderId}_Trigger", "AutoFinishArrivedOrder");
        var jobData = new JobDataMap
        {
            { "orderId", OrderId.ToString() }
        };
        var job = JobBuilder.Create<AutoFinishArrivedOrderJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled auto finish arrived order job for order {OrderId} at {triggerTime.ToLocalTime}");
        return true;
    }
}