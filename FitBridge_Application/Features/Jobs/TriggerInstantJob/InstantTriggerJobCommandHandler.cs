using System;
using FitBridge_Application.Interfaces.Services;
using MediatR;

namespace FitBridge_Application.Features.Jobs.TriggerInstantJob;

public class InstantTriggerJobCommandHandler(IScheduleJobServices _scheduleJobServices) : IRequestHandler<InstantTriggerJobCommand, bool>
{
    public async Task<bool> Handle(InstantTriggerJobCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleJobServices.RescheduleJob($"{request.JobNamePrefix}_{request.KeyId}", request.JobGroup, DateTime.UtcNow.AddSeconds(2));
    }
}
