using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Wallets.GetWalletByUserId;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Dashboards.GetWalletBalance
{
    internal class GetWalletBalanceQueryHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IRequestHandler<GetWalletBalanceQuery, GetWalletBalanceDto>
    {
        public async Task<GetWalletBalanceDto> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetWalletByUserIdSpec(accountId);
            var userWallet = await unitOfWork.Repository<Wallet>()
                .GetBySpecificationAsync(spec)
                ?? throw new NotFoundException(nameof(Wallet));

            return new GetWalletBalanceDto
            {
                TotalPendingBalance = userWallet.PendingBalance,
                TotalAvailableBalance = userWallet.AvailableBalance
            };
        }
    }
}