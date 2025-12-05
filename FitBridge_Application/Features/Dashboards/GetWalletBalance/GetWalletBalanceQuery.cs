using FitBridge_Application.Dtos.Dashboards;
using MediatR;

namespace FitBridge_Application.Features.Dashboards.GetWalletBalance
{
    public class GetWalletBalanceQuery : IRequest<GetWalletBalanceDto>
    {
    }
}