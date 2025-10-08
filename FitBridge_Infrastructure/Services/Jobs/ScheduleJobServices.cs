using System;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Infrastructure.Jobs;
using Quartz;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Services.Jobs;

public class ScheduleJobServices(ISchedulerFactory _schedulerFactory, ILogger<ScheduleJobServices> _logger) : IScheduleJobServices
{
    public async Task<bool> ScheduleProfitDistributionJob(ProfitJobScheduleDto profitJobScheduleDto)
    {
        var jobKey = new JobKey($"ProfitDistribution_{profitJobScheduleDto.CustomerPurchasedId}", "ProfitDistribution");
        var triggerKey = new TriggerKey($"ProfitDistribution_{profitJobScheduleDto.CustomerPurchasedId}_Trigger", "ProfitDistribution");
        var jobData = new JobDataMap
        {
            { "customerPurchasedId", profitJobScheduleDto.CustomerPurchasedId.ToString() }
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
        "Scheduled profit distribution job for CustomerPurchased {CustomerPurchasedId} at {TriggerTime}",
        profitJobScheduleDto.CustomerPurchasedId, triggerTime);
        return true;
    }
}
