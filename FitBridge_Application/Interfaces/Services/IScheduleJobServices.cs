using System;
using FitBridge_Application.Dtos.Jobs;

namespace FitBridge_Application.Interfaces.Services;

public interface IScheduleJobServices
{
    Task<bool> ScheduleProfitDistributionJob(ProfitJobScheduleDto profitJobScheduleDto);

    Task<bool> ScheduleFinishedBookingSession(FinishedBookingSessionJobScheduleDto finishedBookingSessionJobScheduleDto);
    Task<bool> CancelScheduleJob(string jobName, string jobGroup);
}
