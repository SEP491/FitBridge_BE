using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Application.Features.Jobs.RejectEditBookingRequest;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Infrastructure.Jobs;
using FitBridge_Infrastructure.Jobs.BookingRequests;
using FitBridge_Infrastructure.Jobs.Bookings;
using FitBridge_Infrastructure.Jobs.Contracts;
using FitBridge_Infrastructure.Jobs.Orders;
using FitBridge_Infrastructure.Jobs.Reviews;
using FitBridge_Infrastructure.Jobs.Subscriptions;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;

namespace FitBridge_Infrastructure.Services.Jobs;

public class ScheduleJobServices(ISchedulerFactory _schedulerFactory, ILogger<ScheduleJobServices> _logger, SystemConfigurationService _systemConfigurationService) : IScheduleJobServices
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
            return false;
        }
    }

    public async Task<bool> ScheduleAutoCancelBookingJob(Booking booking)
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            var jobKey = new JobKey($"AutoCancelBooking_{booking.Id}", "AutoCancelBooking");
            var triggerKey = new TriggerKey($"AutoCancelBooking_{booking.Id}_Trigger", "AutoCancelBooking");
            var exists = await scheduler.CheckExists(jobKey);
            if (exists)
            {
                _logger.LogWarning("Job for booking {BookingId} already exists. Deleting old job before creating new one.", booking.Id);
                await scheduler.DeleteJob(jobKey);
            }
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
        } catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling auto cancel booking job for booking {BookingId}", booking.Id);
            return false;
        }
    }

    public async Task<bool> ScheduleAutoRejectBookingRequestJob(BookingRequest bookingRequest)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"AutoRejectBookingRequest_{bookingRequest.Id}", "AutoRejectBookingRequest");
        var triggerKey = new TriggerKey($"AutoRejectBookingRequest_{bookingRequest.Id}_Trigger", "AutoRejectBookingRequest");

        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for booking request {BookingRequestId} already exists. Deleting old job before creating new one.", bookingRequest.Id);
            await scheduler.DeleteJob(jobKey);
        }

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
            _logger.LogError("Job {JobName} in {JobGroup} does not exist", jobName, jobGroup);
            return false;
        }
        var triggerKey = new TriggerKey($"{jobName}_Trigger", jobGroup);
        var checkTriggerExists = await scheduler.CheckExists(triggerKey);
        if (!checkTriggerExists)
        {
            _logger.LogError("Trigger {TriggerKey} does not exist", triggerKey);
            return false;
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
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"ExpireUserSubscription_{UserSubscriptionId}", "ExpireUserSubscription");
        var triggerKey = new TriggerKey($"ExpireUserSubscription_{UserSubscriptionId}_Trigger", "ExpireUserSubscription");
        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for user subscription {UserSubscriptionId} already exists. Deleting old job before creating new one.", UserSubscriptionId);
            await scheduler.DeleteJob(jobKey);
        }
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
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"SendRemindExpiredSubscriptionNoti_{UserSubscriptionId}", "SendRemindExpiredSubscriptionNoti");
        var triggerKey = new TriggerKey($"SendRemindExpiredSubscriptionNoti_{UserSubscriptionId}_Trigger", "SendRemindExpiredSubscriptionNoti");
        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for user subscription {UserSubscriptionId} already exists. Deleting old job before creating new one.", UserSubscriptionId);
            await scheduler.DeleteJob(jobKey);
        }
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
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"AutoRejectEditBookingRequest_{BookingRequestId}", "AutoRejectEditBookingRequest");
        var triggerKey = new TriggerKey($"AutoRejectEditBookingRequest_{BookingRequestId}_Trigger", "AutoRejectEditBookingRequest");
        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for booking request {BookingRequestId} already exists. Deleting old job before creating new one.", BookingRequestId);
            await scheduler.DeleteJob(jobKey);
        }
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
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"AutoFinishArrivedOrder_{OrderId}", "AutoFinishArrivedOrder");
        var triggerKey = new TriggerKey($"AutoFinishArrivedOrder_{OrderId}_Trigger", "AutoFinishArrivedOrder");
        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for order {OrderId} already exists. Deleting old job before creating new one.", OrderId);
            await scheduler.DeleteJob(jobKey);
        }
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

    public async Task<bool> ScheduleAutoMarkAsFeedbackJob(Guid OrderItemId, DateTime triggerTime)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"AutoMarkAsFeedback_{OrderItemId}", "AutoMarkAsFeedback");
        var triggerKey = new TriggerKey($"AutoMarkAsFeedback_{OrderItemId}_Trigger", "AutoMarkAsFeedback");
        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for order item {OrderItemId} already exists. Deleting old job before creating new one.", OrderItemId);
            await scheduler.DeleteJob(jobKey);
        }
        var jobData = new JobDataMap
        {
            { "orderItemId", OrderItemId.ToString() }
        };
        var job = JobBuilder.Create<MarkAsFeedbackJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled auto mark as reviewed job for order item {OrderItemId} at {triggerTime.ToLocalTime}");
        return true;
    }

    public async Task<bool> ScheduleAutoCancelCreatedOrderJob(Guid orderId)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"AutoCancelCreatedOrder_{orderId}", "AutoCancelCreatedOrder");
        var triggerKey = new TriggerKey($"AutoCancelCreatedOrder_{orderId}_Trigger", "AutoCancelCreatedOrder");
        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for order {OrderId} already exists. Deleting old job before creating new one.", orderId);
            await scheduler.DeleteJob(jobKey);
        }
        var jobData = new JobDataMap
        {
            { "orderId", orderId.ToString() }
        };
        var expirationMinutes = (int)await _systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.AutoCancelCreatedOrderAfterTime);
        var triggerTime = DateTime.Now.AddMinutes(expirationMinutes);
        var job = JobBuilder.Create<CancelCreatedOrderJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled auto return quantity to product detail job for product detail of order Id {orderId} at {triggerTime.ToLocalTime}");
        return true;
    }

    public async Task<bool> ScheduleAutoUpdatePTCurrentCourseJob(Guid OrderItemId, DateOnly expirationDate)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"AutoUpdatePTCurrentCourse_{OrderItemId}", "AutoUpdatePTCurrentCourse");
        var triggerKey = new TriggerKey($"AutoUpdatePTCurrentCourse_{OrderItemId}_Trigger", "AutoUpdatePTCurrentCourse");
        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for order item {OrderItemId} already exists. Deleting old job before creating new one.", OrderItemId);
            await scheduler.DeleteJob(jobKey);
        }
        var jobData = new JobDataMap
        {
            { "orderItemId", OrderItemId.ToString() }
        };
        var triggerTime = expirationDate.ToDateTime(TimeOnly.MaxValue);
        var job = JobBuilder.Create<UpdatePTCurrentCourseJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled auto update PT current course job for order item {OrderItemId} at {triggerTime.ToLocalTime}");
        return true;
    }

    public async Task<bool> ScheduleAutoExpiredContractAccountJob(Guid ContractId, DateTime triggerTime)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"AutoExpiredContractAccount_{ContractId}", "AutoExpiredContractAccount");
        var triggerKey = new TriggerKey($"AutoExpiredContractAccount_{ContractId}_Trigger", "AutoExpiredContractAccount");
        var exists = await scheduler.CheckExists(jobKey);
        if (exists)
        {
            _logger.LogWarning("Job for contract {ContractId} already exists. Deleting old job before creating new one.", ContractId);
            await scheduler.DeleteJob(jobKey);
        }
        var jobData = new JobDataMap
        {
            { "contractId", ContractId.ToString() }
        };
        var job = JobBuilder.Create<ExpireContractAccountJob>()
        .WithIdentity(jobKey)
        .SetJobData(jobData)
        .Build();
        var trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(triggerTime)
        .Build();
        await _schedulerFactory.GetScheduler().Result.ScheduleJob(job, trigger);
        _logger.LogInformation($"Successfully scheduled auto expired contract account job for contract {ContractId} at {triggerTime.ToLocalTime}");
        return true;
    }
}