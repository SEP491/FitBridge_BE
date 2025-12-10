using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Dashboards.GetTransactionForAvailableBalanceDetail;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Dashboards.GetAvailableBalanceDetail
{
    internal class GetAvailableBalanceDetailQueryHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IRequestHandler<GetAvailableBalanceDetailQuery, DashboardPagingResultDto<AvailableBalanceTransactionDto>>
    {
        public async Task<DashboardPagingResultDto<AvailableBalanceTransactionDto>> Handle(GetAvailableBalanceDetailQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var accountRole = userUtil.GetUserRole(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException("User role");

            // Get the current wallet to show the balance
            var userWallet = await unitOfWork.Repository<Wallet>()
                .GetByIdAsync(accountId)
                ?? throw new NotFoundException(nameof(Wallet));

            var transactionSpec = new GetTransactionForAvailableBalanceDetailSpec(accountId, request.Params);
            var transactions = await unitOfWork.Repository<Transaction>()
                .GetAllWithSpecificationAsync(transactionSpec);

            var countSpec = new GetTransactionForAvailableBalanceDetailSpec(accountId, request.Params);
            var totalCount = await unitOfWork.Repository<Transaction>()
                .CountAsync(countSpec);

            var mappedTransactions = transactions.Select(transaction =>
            {
                // handle withdraw & profit distribution transactions
                var isWithdrawal = transaction.TransactionType == TransactionType.Withdraw;
                var isGymOwner = accountRole == ProjectConstant.UserRoles.GymOwner;

                string? GetCourseName()
                {
                    if (isWithdrawal) return null;

                    if (isGymOwner)
                    {
                        return transaction.OrderItem!.GymCourse!.Name;
                    }
                    else
                    {
                        return transaction.OrderItem!.FreelancePTPackage!.Name;
                    }
                }

                return new AvailableBalanceTransactionDto
                {
                    OrderItemId = isWithdrawal ? null : transaction.OrderItemId!.Value,
                    CourseName = GetCourseName(),
                    TransactionId = transaction.Id,
                    TotalProfit = isWithdrawal ? transaction.Amount : transaction.Amount * -1,
                    TransactionType = transaction.TransactionType.ToString(),
                    ActualDistributionDate = isWithdrawal ? null : transaction.OrderItem!.ProfitDistributeActualDate,
                    WithdrawDate = isWithdrawal && transaction.WithdrawalRequest != null ? transaction.WithdrawalRequest.CreatedAt : null, // by the time admin approved
                    WithdrawalRequestId = isWithdrawal ? transaction.WithdrawalRequestId : null,
                    Description = transaction.Description
                };
            }).ToList();

            var totalProfitSum = mappedTransactions.Sum(t => t.TotalProfit);

            return new DashboardPagingResultDto<AvailableBalanceTransactionDto>(totalCount, mappedTransactions, totalProfitSum);
        }
    }
}