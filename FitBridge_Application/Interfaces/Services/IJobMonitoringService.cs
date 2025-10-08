using System;
using FitBridge_Application.Dtos.Jobs;

namespace FitBridge_Application.Interfaces.Services;

public interface IJobMonitoringService
{
    Task<List<JobInfoDto>> GetAllScheduledJobsAsync();
}
