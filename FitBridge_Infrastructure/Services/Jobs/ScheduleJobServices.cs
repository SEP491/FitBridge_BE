using System;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Application.Interfaces.Services;
using Quartz;
using Microsoft.Extensions.Logging;
using FitBridge_Infrastructure.Jobs;

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
}
