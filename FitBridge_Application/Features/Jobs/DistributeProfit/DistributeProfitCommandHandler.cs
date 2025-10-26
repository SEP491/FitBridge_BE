using System;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Application.Interfaces.Services;
using MediatR;

namespace FitBridge_Application.Features.Jobs.DistributeProfit;

public class DistributeProfitCommandHandler(IScheduleJobServices _scheduleJobServices) : IRequestHandler<DistributeProfitCommand, bool>
{
    public async Task<bool> Handle(DistributeProfitCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleJobServices.ScheduleProfitDistributionJob(new ProfitJobScheduleDto
        {
            OrderItemId = request.OrderItemId,
            ProfitDistributionDate = request.ProfitDistributionDate
        });
    }

}
