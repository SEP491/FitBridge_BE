using System;
using MediatR;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Transactions.GetGymOwnerTransactionById;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Features.Transactions.GetGymOwnerTransactionById;

public class GetGymOwnerTransactionByIdCommandHandler(IUserUtil userUtil, IHttpContextAccessor httpContextAccessor, IUnitOfWork _unitOfWork, ITransactionService _transactionService) : IRequestHandler<GetGymOwnerTransactionByIdCommand, MerchantTransactionDetailDto>
{
    public async Task<MerchantTransactionDetailDto> Handle(GetGymOwnerTransactionByIdCommand request, CancellationToken cancellationToken)
    {
        var userId = userUtil.GetAccountId(httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));
        var spec = new GetGymOwnerTransactionByIdSpec(request.TransactionId);
        var transaction = await _unitOfWork.Repository<Transaction>().GetBySpecificationAsync(spec) ?? throw new NotFoundException(nameof(Transaction), request.TransactionId);
        var merchantTransactionDetailDto = new MerchantTransactionDetailDto
        {
            TransactionId = transaction.Id,
            TransactionType = transaction.TransactionType,
            TotalPaidAmount = transaction.TransactionType == TransactionType.SubscriptionPlansOrder ? transaction.Amount : null,
            OrderCode = transaction.OrderCode,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            PaymentMethod = transaction.PaymentMethod.MethodType.ToString(),
            CustomerName = transaction.Order.Account.FullName,
            CustomerId = transaction.Order.AccountId,
            CustomerAvatarUrl = transaction.Order.Account.AvatarUrl,
        };
            if (transaction.TransactionType != TransactionType.SubscriptionPlansOrder && transaction.TransactionType != TransactionType.DistributeProfit)
            {
                var profitAmount = await CalculateProfitAmount(transaction, userId);
                merchantTransactionDetailDto.ProfitAmount = profitAmount;
                merchantTransactionDetailDto.OrderItemId = transaction.Order.OrderItems.FirstOrDefault(x => x.GymCourseId != null && x.GymCourse.GymOwnerId == userId)?.Id ?? null;
            }
            if (transaction.TransactionType == TransactionType.DistributeProfit)
            {
                merchantTransactionDetailDto.ProfitAmount = transaction.Amount;
                merchantTransactionDetailDto.OrderItemId = transaction.OrderItemId ?? null;
            }
            if(transaction.TransactionType == TransactionType.Withdraw)
            {
                merchantTransactionDetailDto.WithdrawalAmount = transaction.Amount;
            }
        return merchantTransactionDetailDto;
    }
        public async Task<decimal> CalculateProfitAmount(Transaction transaction, Guid gymOwnerId)
    {
        var orderItem = transaction.Order.OrderItems.FirstOrDefault(x => x.GymCourseId != null && x.GymCourse.GymOwnerId == gymOwnerId);
        if (orderItem == null)
        {
            throw new NotFoundException("Order item not found");
        }
        var profitAmount = await _transactionService.CalculateMerchantProfit(orderItem, transaction.Order.Coupon);
        return profitAmount;
    }

}
