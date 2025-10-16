using System;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Application.Interfaces.Services;
using Quartz;
using Microsoft.Extensions.Logging;
using FitBridge_Infrastructure.Jobs;
using FitBridge_Infrastructure.Jobs.Bookings;
using FitBridge_Domain.Exceptions;

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
}
