using System;
using MediatR;

namespace FitBridge_Application.Features.Jobs.DistributeProfit;

public class DistributeProfitCommand : IRequest<bool>
{
    public Guid OrderItemId { get; set; }
    public DateOnly ProfitDistributionDate { get; set; }
}
