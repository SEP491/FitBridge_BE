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
using FitBridge_Application.Dtos.Payments;

namespace FitBridge_Application.Services;

public class TransactionsService(IUnitOfWork _unitOfWork, ILogger<TransactionsService> _logger, ISchedulerFactory _schedulerFactory, IScheduleJobServices _scheduleJobServices, IApplicationUserService _applicationUserService) : ITransactionService
{
    public async Task<bool> ExtendCourse(long orderCode)
    {
        var transactionToExtend = await _unitOfWork.Repository<FitBridge_Domain.Entities.Orders.Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeWithIncludeSpec(orderCode), false);
        if (transactionToExtend == null)
        {
            throw new NotFoundException("Transaction not found with order code " + orderCode);
        }
        if (transactionToExtend.Order.Coupon != null)
        {
            transactionToExtend.Order.Coupon.Quantity--;
            transactionToExtend.Order.Coupon.NumberOfUsedCoupon++;
        }
        transactionToExtend.ProfitAmount = await CalculateSystemProfit(transactionToExtend.Order);
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
        var profit = await CalculateMerchantProfit(orderItemToExtend, transactionToExtend.Order.Coupon);
        walletToUpdate.PendingBalance += profit;
        _logger.LogInformation($"Wallet {walletToUpdate.Id} updated with new pending balance {walletToUpdate.PendingBalance} after adding profit {profit}");
        transactionToExtend.ProfitAmount = profit;

        _unitOfWork.Repository<Wallet>().Update(walletToUpdate);
        await _unitOfWork.CommitAsync();

        await _scheduleJobServices.ScheduleProfitDistributionJob(new ProfitJobScheduleDto
        {
            OrderItemId = orderItemToExtend.Id,
            ProfitDistributionDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30)
        });
        return true;
    }
    public async Task<decimal> CalculateSystemProfit(Order order)
    {
        var systemProfit = order.SubTotalPrice * ProjectConstant.CommissionRate;
        if (order.Coupon != null)
        {
            if (order.Coupon.Type == CouponType.FreelancePT)
            {
                systemProfit = order.TotalAmount * ProjectConstant.CommissionRate;
            }
            else if (order.Coupon.Type == CouponType.System)
            {
                var discountAmount = order.SubTotalPrice * (decimal)(order.Coupon.DiscountPercent / 100) > order.Coupon.MaxDiscount ? order.Coupon.MaxDiscount : order.SubTotalPrice * (decimal)(order.Coupon.DiscountPercent / 100);
                systemProfit = (order.SubTotalPrice * ProjectConstant.CommissionRate) - discountAmount;
            }
        }
        return Math.Round(systemProfit, 0, MidpointRounding.AwayFromZero);
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
        var orderItem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(orderItemId, includes: new List<string> { "Order", "FreelancePTPackage", "GymCourse", "Order.Coupon", "Order.Coupon.Creator" });

        if (orderItem == null)
        {
            throw new NotFoundException($"{nameof(orderItem)} with Id {orderItemId} not found");
        }
        var profit = await CalculateMerchantProfit(orderItem, orderItem.Order.Coupon);

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
        OrderEntity.Transactions.FirstOrDefault(t => t.OrderCode == orderCode).ProfitAmount = await CalculateSystemProfit(OrderEntity);

        if (OrderEntity.Coupon != null)
        {
            OrderEntity.Coupon.Quantity--;
            OrderEntity.Coupon.NumberOfUsedCoupon++;
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
            var profit = await CalculateMerchantProfit(orderItem, OrderEntity.Coupon);
            walletToUpdate.PendingBalance += profit;


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

    private async Task<decimal> CalculateMerchantProfit(OrderItem orderItem, Coupon? coupon)
    {
        var subTotalOrderItemPrice = orderItem.Price * orderItem.Quantity;
        var commissionAmount = subTotalOrderItemPrice * ProjectConstant.CommissionRate;
        var merchantPtProfit = Math.Round(subTotalOrderItemPrice - commissionAmount, 0, MidpointRounding.AwayFromZero);
        if (coupon != null) // If there is a voucher, recalculate the profit
        {
            var voucherOwnerRoles = await _applicationUserService.GetUserRoleAsync(coupon.Creator);
            if (voucherOwnerRoles == ProjectConstant.UserRoles.FreelancePT)
            {
                var discountAmount = subTotalOrderItemPrice * (decimal)(coupon.DiscountPercent / 100) > coupon.MaxDiscount ? coupon.MaxDiscount : subTotalOrderItemPrice * (decimal)(coupon.DiscountPercent / 100);
                merchantPtProfit = merchantPtProfit - discountAmount;
            }
        }

        return Math.Round(merchantPtProfit, 0, MidpointRounding.AwayFromZero);

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
        if (OrderEntity.Coupon != null)
        {
            OrderEntity.Coupon.Quantity--;
            OrderEntity.Coupon.NumberOfUsedCoupon++;
            _unitOfWork.Repository<Coupon>().Update(OrderEntity.Coupon);
        }
        OrderEntity.Transactions.FirstOrDefault(t => t.OrderCode == orderCode).ProfitAmount = await CalculateSystemProfit(OrderEntity);
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
                var profit = await CalculateMerchantProfit(orderItem, OrderEntity.Coupon);
                walletToUpdate.PendingBalance += profit;
                OrderEntity.Transactions.FirstOrDefault(t => t.OrderCode == orderCode).ProfitAmount = profit;
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
