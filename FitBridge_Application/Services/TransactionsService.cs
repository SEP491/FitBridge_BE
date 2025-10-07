using System;
using FitBridge_Application.Specifications.Transactions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymCoursePts.GetGymCoursePtById;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Application.Commons.Constants;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging;
using FitBridge_Application.Commons.Utils;

namespace FitBridge_Application.Services;

public class TransactionsService(IUnitOfWork _unitOfWork, ILogger<TransactionsService> _logger) : ITransactionService
{
    public async Task<bool> ExtendCourse(long orderCode)
    {
        var transactionToExtend = await _unitOfWork.Repository<FitBridge_Domain.Entities.Orders.Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeWithIncludeSpec(orderCode), false);
        if (transactionToExtend == null)
        {
            throw new NotFoundException("Transaction not found with order code " + orderCode);
        }
        var orderItemToExtend = transactionToExtend.Order.OrderItems.First();
        var customerPurchasedToExtend = transactionToExtend.Order.CustomerPurchasedToExtend;
        orderItemToExtend.CustomerPurchasedId = customerPurchasedToExtend.Id;
        var numOfSession = 0;
        if (orderItemToExtend.FreelancePTPackageId != null)
        {
            numOfSession = orderItemToExtend.FreelancePTPackage.NumOfSessions;
        }

        if (orderItemToExtend.GymCourseId != null && orderItemToExtend.GymPtId != null)
        {
            var gymCoursePT = await _unitOfWork.Repository<GymCoursePT>().GetBySpecificationAsync(new GetGymCoursePtByGymCourseIdAndPtIdSpec(orderItemToExtend.GymCourseId.Value, orderItemToExtend.GymPtId.Value));
            if (gymCoursePT == null)
            {
                throw new NotFoundException("Gym course PT with gym course id and pt id not found");
            }

            numOfSession = gymCoursePT.Session.Value;
        }
        customerPurchasedToExtend.AvailableSessions += orderItemToExtend.Quantity * numOfSession;
        customerPurchasedToExtend.ExpirationDate = customerPurchasedToExtend.ExpirationDate.AddDays(orderItemToExtend.GymCourse.Duration * orderItemToExtend.Quantity);
        transactionToExtend.Order.Status = OrderStatus.Arrived;

        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> PurchasePt(long orderCode)
    {
        var transactionEntity = await _unitOfWork.Repository<FitBridge_Domain.Entities.Orders.Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeWithIncludeSpec(orderCode), false);
        if (transactionEntity == null)
        {
            throw new NotFoundException("Transaction not found with order code " + orderCode);
        }
        var customerPurchasedToAssignPt = transactionEntity.Order.CustomerPurchasedToExtend;
        var orderItemToAssignPt = customerPurchasedToAssignPt.OrderItems.OrderByDescending(x => x.CreatedAt).First();

        var gymCoursePTToAssign = transactionEntity.Order.GymCoursePTToAssign;
        var numOfSession = gymCoursePTToAssign.Session;

        customerPurchasedToAssignPt.AvailableSessions = numOfSession.Value;
        orderItemToAssignPt.GymPtId = gymCoursePTToAssign.PTId;

        transactionEntity.Order.Status = OrderStatus.Arrived;

        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> DistributeProfit(Guid customerPurchasedId)
    {
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(customerPurchasedId, includes: new List<string> { "OrderItems" });

        if (customerPurchased == null)
        {
            throw new NotFoundException($"{nameof(CustomerPurchased)} not found");
        }
        var orderItem = customerPurchased.OrderItems.OrderByDescending(o => o.Order.CreatedAt).FirstOrDefault();
        var profit = orderItem.Price * orderItem.Quantity * ProjectConstant.CommissionRate;

        var DistributeProfTransaction = new Transaction
        {
            Amount = profit,
            OrderId = orderItem.OrderId,
            TransactionType = TransactionType.DistributeProfit,
            Status = TransactionStatus.Success,
            Description = $"Profit distribution for completed course - CustomerPurchased: {customerPurchasedId}",
            OrderCode = GenerateOrderCode(),
            PaymentMethodId = await GetSystemPaymentMethodId.GetPaymentMethodId(MethodType.System, _unitOfWork)
        };
        _unitOfWork.Repository<Transaction>().Insert(DistributeProfTransaction);


        var wallet = await _unitOfWork.Repository<Wallet>().GetByIdAsync(customerPurchased.CustomerId);
        if (wallet == null)
        {
            throw new NotFoundException($"{nameof(Wallet)} not found");
        }
        _logger.LogInformation($"Wallet {wallet.Id} updated with available balance {wallet.AvailableBalance} plus {profit} and pending balance {wallet.PendingBalance} minus {profit}");
        wallet.AvailableBalance += profit;
        wallet.PendingBalance -= profit;

        _unitOfWork.Repository<Wallet>().Update(wallet);
        await _unitOfWork.CommitAsync();
        return true;
    }
    
    private long GenerateOrderCode()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
