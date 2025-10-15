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
using FitBridge_Application.Specifications.Orders;
using Quartz;
using FitBridge_Application.Dtos.Jobs;

namespace FitBridge_Application.Services;

public class TransactionsService(IUnitOfWork _unitOfWork, ILogger<TransactionsService> _logger, ISchedulerFactory _schedulerFactory, IScheduleJobServices _scheduleJobServices) : ITransactionService
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
        var walletToUpdate = await _unitOfWork.Repository<Wallet>().GetByIdAsync(orderItemToExtend.GymCourse.GymOwnerId);
        if (walletToUpdate == null)
        {
            throw new NotFoundException("Wallet not found");
        }
        walletToUpdate.PendingBalance += orderItemToExtend.Price * orderItemToExtend.Quantity * ProjectConstant.CommissionRate;
        _unitOfWork.Repository<Wallet>().Update(walletToUpdate);
        await _unitOfWork.CommitAsync();

        await _scheduleJobServices.ScheduleProfitDistributionJob(new ProfitJobScheduleDto
        {
            OrderItemId = orderItemToExtend.Id,
            ProfitDistributionDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30)
        });
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

    public async Task<bool> DistributeProfit(Guid orderItemId)
    {
        var orderItem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(orderItemId, includes: new List<string> { "Order", "FreelancePTPackage", "GymCourse" });

        if (orderItem == null)
        {
            throw new NotFoundException($"{nameof(orderItem)} with Id {orderItemId} not found");
        }
        var profit = orderItem.Price * orderItem.Quantity * ProjectConstant.CommissionRate;

        var DistributeProfTransaction = new Transaction
        {
            Amount = profit,
            OrderId = orderItem.OrderId,
            TransactionType = TransactionType.DistributeProfit,
            Status = TransactionStatus.Success,
            Description = $"Profit distribution for completed course - OrderItem: {orderItemId}",
            OrderCode = GenerateOrderCode(),
            PaymentMethodId = await GetSystemPaymentMethodId.GetPaymentMethodId(MethodType.System, _unitOfWork)
        };
        _unitOfWork.Repository<Transaction>().Insert(DistributeProfTransaction);

        Wallet wallet = null;
        if (orderItem.FreelancePTPackageId != null)
        {
            wallet = await _unitOfWork.Repository<Wallet>().GetByIdAsync(orderItem.FreelancePTPackage.PtId);
        }
        if (orderItem.GymCourseId != null)
        {
            wallet = await _unitOfWork.Repository<Wallet>().GetByIdAsync(orderItem.GymCourse.GymOwnerId);
        }
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

    public async Task<bool> PurchaseFreelancePTPackage(long orderCode)
    {
        var OrderEntity = await _unitOfWork.Repository<Order>()
            .GetBySpecificationAsync(new GetOrderByOrderCodeSpecification(orderCode), false);
        if (OrderEntity == null)
        {
            throw new NotFoundException("Order not found");
        }
        if (OrderEntity.OrderItems.Any(item => item.ProductDetailId != null))
        {
            OrderEntity.Status = OrderStatus.Pending;
        }
        else
        {
            OrderEntity.Status = OrderStatus.Arrived;
        }
        foreach (var orderItem in OrderEntity.OrderItems)
        {
            var expirationDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var numOfSession = 0;
            var profitDistributionDate = DateOnly.FromDateTime(DateTime.UtcNow);
            if (orderItem.FreelancePTPackageId == null)
            {
                throw new NotFoundException("Freelance PTPackage Id in order item not found");
            }

            numOfSession = orderItem.FreelancePTPackage.NumOfSessions;
            expirationDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(orderItem.FreelancePTPackage.DurationInDays * orderItem.Quantity);
            profitDistributionDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(orderItem.FreelancePTPackage.DurationInDays * orderItem.Quantity);

            orderItem.CustomerPurchased = new CustomerPurchased
            {
                CustomerId = OrderEntity.AccountId,
                AvailableSessions = orderItem.Quantity * numOfSession,
                ExpirationDate = expirationDate,
            };
            var walletToUpdate = await _unitOfWork.Repository<Wallet>().GetByIdAsync(orderItem.FreelancePTPackage.PtId);
            if (walletToUpdate == null)
            {
                throw new NotFoundException("Wallet not found");
            }
            walletToUpdate.PendingBalance += orderItem.Price * orderItem.Quantity * ProjectConstant.CommissionRate;
            _unitOfWork.Repository<Wallet>().Update(walletToUpdate);
            await _unitOfWork.CommitAsync();

            await _scheduleJobServices.ScheduleProfitDistributionJob(new ProfitJobScheduleDto
            {
                OrderItemId = orderItem.Id,
                ProfitDistributionDate = profitDistributionDate
            });
            
        }
        return true;
    }

    public async Task<bool> PurchaseGymCourse(long orderCode)
    {
        var OrderEntity = await _unitOfWork.Repository<Order>()
                .GetBySpecificationAsync(new GetOrderByOrderCodeSpecification(orderCode), false);
            if (OrderEntity == null)
            {
                throw new NotFoundException("Order not found");
            }
            if (OrderEntity.OrderItems.Any(item => item.ProductDetailId != null))
            {
                OrderEntity.Status = OrderStatus.Pending;
            }
            else
            {
                OrderEntity.Status = OrderStatus.Arrived;
            }
            foreach (var orderItem in OrderEntity.OrderItems)
            {
                if (orderItem.ProductDetailId == null)
                {
                    var expirationDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    var numOfSession = 0;
                    var profitDistributionDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30);
                    if (orderItem.GymCourseId != null && orderItem.GymPtId != null)
                    {
                        var gymCoursePT = await _unitOfWork.Repository<GymCoursePT>().GetBySpecificationAsync(new GetGymCoursePtByGymCourseIdAndPtIdSpec(orderItem.GymCourseId.Value, orderItem.GymPtId.Value));
                        if (gymCoursePT == null)
                        {
                            throw new NotFoundException("Gym course PT with gym course id and pt id not found");
                        }
                        numOfSession = gymCoursePT.Session.Value;
                    }

                    expirationDate = expirationDate.AddDays(orderItem.GymCourse.Duration * orderItem.Quantity);
                    orderItem.CustomerPurchased = new CustomerPurchased
                    {
                        CustomerId = OrderEntity.AccountId,
                        AvailableSessions = orderItem.Quantity * numOfSession,
                        ExpirationDate = expirationDate,
                    };
                    var walletToUpdate = await _unitOfWork.Repository<Wallet>().GetByIdAsync(orderItem.GymCourse.GymOwnerId);
                    if (walletToUpdate == null)
                    {
                        throw new NotFoundException("Wallet not found");
                    }
                    walletToUpdate.PendingBalance += orderItem.Price * orderItem.Quantity * ProjectConstant.CommissionRate;
                    _unitOfWork.Repository<Wallet>().Update(walletToUpdate);
                    await _unitOfWork.CommitAsync();

                    await _scheduleJobServices.ScheduleProfitDistributionJob(new ProfitJobScheduleDto
                    {
                        OrderItemId = orderItem.Id,
                        ProfitDistributionDate = profitDistributionDate
                    });
                }
            }
            return true;
    }
}
