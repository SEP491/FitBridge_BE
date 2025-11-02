using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using MediatR;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Specifications.Transactions.GetAllGymOwnerTransaction;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Transactions.GetAllGymOwnerTransaction;

public class GetAllGymOwnerTransactionQueryHandler(IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, IMapper _mapper, ITransactionService _transactionService) : IRequestHandler<GetAllGymOwnerTransactionQuery, PagingResultDto<GetAllMerchantTransactionDto>>
{
    public async Task<PagingResultDto<GetAllMerchantTransactionDto>> Handle(GetAllGymOwnerTransactionQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext) ?? throw new NotFoundException(nameof(ApplicationUser));
        var spec = new GetAllGymOwnerTransactionSpec(request.Parameters, userId);
        var transactions = await _unitOfWork.Repository<Transaction>().GetAllWithSpecificationAsync(spec);
        var result = new List<GetAllMerchantTransactionDto>();
        foreach (var transaction in transactions)
        {
            var getAllMerchantTransactionDto = new GetAllMerchantTransactionDto
            {
                TransactionId = transaction.Id,
                TransactionType = transaction.TransactionType,
                TotalPaidAmount = transaction.TransactionType == TransactionType.PurchasePremiumService ? transaction.Amount : null,
                OrderCode = transaction.OrderCode,
                CustomerName = transaction.Order.Account.FullName,
                CustomerAvatarUrl = transaction.Order.Account.AvatarUrl,
            };
            if (transaction.TransactionType != TransactionType.PurchasePremiumService && transaction.TransactionType != TransactionType.DistributeProfit)
            {
                var profitAmount = await CalculateProfitAmount(transaction, userId);
                getAllMerchantTransactionDto.ProfitAmount = profitAmount;
            }
            if (transaction.TransactionType == TransactionType.DistributeProfit)
            {
                getAllMerchantTransactionDto.ProfitAmount = transaction.Amount;
            }   
            if(transaction.TransactionType == TransactionType.Withdraw)
            {
                getAllMerchantTransactionDto.WithdrawalAmount = transaction.Amount;
            }
            result.Add(getAllMerchantTransactionDto);
        }
        var totalCount = await _unitOfWork.Repository<Transaction>().CountAsync(spec);
        return new PagingResultDto<GetAllMerchantTransactionDto>(totalCount, result);
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
