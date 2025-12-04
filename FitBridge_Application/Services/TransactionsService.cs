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
using FitBridge_Domain.Enums.Trainings;
using Microsoft.VisualBasic;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Enums.SubscriptionPlans;
using FitBridge_Application.Dtos.Payments.ApplePaymentDto;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Services;

public class TransactionsService(IUnitOfWork _unitOfWork, ILogger<TransactionsService> _logger, ISchedulerFactory _schedulerFactory, IScheduleJobServices _scheduleJobServices, IApplicationUserService _applicationUserService, SystemConfigurationService systemConfigurationService) : ITransactionService
{
    private int defaultProfitDistributionDays;
    private decimal defaultCommissionRate;
    private int autoMarkAsFeedbackAfterDays;
    public async Task<int> GetProfitDistributionDays()
    {
        if (defaultProfitDistributionDays == 0)
        {
            defaultProfitDistributionDays = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.ProfitDistributionDays);
        }
        return defaultProfitDistributionDays;
    }
    public async Task<decimal> GetCommissionRate()
    {
        if (defaultCommissionRate == 0)
        {
            defaultCommissionRate = (decimal)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.CommissionRate);
        }
        return defaultCommissionRate;
    }
    public async Task<int> GetAutoMarkAsFeedbackAfterDays()
    {
        if (autoMarkAsFeedbackAfterDays == 0)
        {
            autoMarkAsFeedbackAfterDays = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.AutoMarkAsFeedbackAfterDays);
        }
        return autoMarkAsFeedbackAfterDays;
    }
    public async Task<bool> ExtendCourse(long orderCode)
    {
        autoMarkAsFeedbackAfterDays = await GetAutoMarkAsFeedbackAfterDays();
        defaultProfitDistributionDays = await GetProfitDistributionDays();
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
        var profitDistributionDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(defaultProfitDistributionDays); // Profit distribute planned date is the day after the expiration date
        orderItemToExtend.ProfitDistributePlannedDate = profitDistributionDate;
        transactionToExtend.Order.Status = OrderStatus.Finished;
        var walletToUpdate = await _unitOfWork.Repository<Wallet>().GetByIdAsync(orderItemToExtend.GymCourse.GymOwnerId);
        if (walletToUpdate == null)
        {
            throw new NotFoundException("Wallet not found");
        }
        var profit = await CalculateMerchantProfit(orderItemToExtend, transactionToExtend.Order.Coupon);
        walletToUpdate.PendingBalance += profit;
         transactionToExtend.ProfitAmount = profit;

        _unitOfWork.Repository<Wallet>().Update(walletToUpdate);
        await _unitOfWork.CommitAsync();

        await _scheduleJobServices.ScheduleProfitDistributionJob(new ProfitJobScheduleDto
        {
            OrderItemId = orderItemToExtend.Id,
            ProfitDistributionDate = profitDistributionDate
        });
        var originalCustomerPurchasedOrderItem = customerPurchasedToExtend.OrderItems.OrderBy(o => o.CreatedAt).First();

        await _scheduleJobServices.RescheduleJob($"AutoUpdatePTCurrentCourse_{originalCustomerPurchasedOrderItem.Id}", "AutoUpdatePTCurrentCourse", customerPurchasedToExtend.ExpirationDate.ToDateTime(TimeOnly.MaxValue));

        await _scheduleJobServices.CancelScheduleJob($"AutoCancelCreatedOrder_{transactionToExtend.Order.Id}", "AutoCancelCreatedOrder");
        return true;
    }

    public async Task<decimal> CalculateSystemProfit(Order order)
    {
        var commissionRate = order.CommissionRate;
        var systemProfit = order.SubTotalPrice * commissionRate;
        if (order.Coupon != null)
        {
            if (order.Coupon.Type == CouponType.FreelancePT || order.Coupon.Type == CouponType.GymOwner)
            {
                systemProfit = order.TotalAmount * commissionRate;
            }
            else if (order.Coupon.Type == CouponType.System)
            {
                var discountAmount = order.SubTotalPrice * (decimal)(order.Coupon.DiscountPercent / 100) > order.Coupon.MaxDiscount ? order.Coupon.MaxDiscount : order.SubTotalPrice * (decimal)(order.Coupon.DiscountPercent / 100);
                systemProfit = (order.SubTotalPrice * commissionRate) - discountAmount;
            }
        }
        return Math.Round(systemProfit, 0, MidpointRounding.AwayFromZero);
    }

    public async Task<decimal> CalculateCommissionAmount(OrderItem orderItem, Coupon? coupon)
    {
        var subTotalPrice = orderItem.Price * orderItem.Quantity;
        var commissionAmount = subTotalPrice * orderItem.Order.CommissionRate;
        if (coupon != null)
        {
            if (coupon.Type != CouponType.System)
            {
                var discountAmount = subTotalPrice * (decimal)(coupon.DiscountPercent / 100) > coupon.MaxDiscount ? coupon.MaxDiscount : subTotalPrice * (decimal)(coupon.DiscountPercent / 100);
                commissionAmount = (subTotalPrice - discountAmount) * orderItem.Order.CommissionRate;
            }
            else
            {
                commissionAmount = subTotalPrice * orderItem.Order.CommissionRate;
            }
        }
        return Math.Round(commissionAmount, 0, MidpointRounding.AwayFromZero);
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

        transactionEntity.Order.Status = OrderStatus.Finished;

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
            WalletId = orderItem.GymCourseId != null ? orderItem.GymCourse.GymOwnerId : orderItem.FreelancePTPackage.PtId,
            OrderId = orderItem.OrderId,
            OrderItemId = orderItemId,
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
        orderItem.ProfitDistributeActualDate = DateOnly.FromDateTime(DateTime.UtcNow); // Profit distribute actual date is the current date
        orderItem.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<OrderItem>().Update(orderItem);
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
        autoMarkAsFeedbackAfterDays = await GetAutoMarkAsFeedbackAfterDays();
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
            OrderEntity.Status = OrderStatus.Finished;
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
            profitDistributionDate = expirationDate.AddDays(1); // Profit distribute planned date is the day after the expiration date
            orderItem.ProfitDistributePlannedDate = profitDistributionDate;

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
            await _scheduleJobServices.ScheduleAutoUpdatePTCurrentCourseJob(orderItem.Id, expirationDate);

            await _scheduleJobServices.ScheduleAutoMarkAsFeedbackJob(orderItem.Id, profitDistributionDate.AddDays(autoMarkAsFeedbackAfterDays).ToDateTime(TimeOnly.MinValue));
        }
        await _scheduleJobServices.CancelScheduleJob($"AutoCancelCreatedOrder_{OrderEntity.Id}", "AutoCancelCreatedOrder");

        return true;
    }

    public async Task<decimal> CalculateMerchantProfit(OrderItem orderItem, Coupon? coupon)
    {
        var subTotalOrderItemPrice = orderItem.Price * orderItem.Quantity;
        var commissionAmount = subTotalOrderItemPrice * orderItem.Order.CommissionRate;
        var merchantPtProfit = Math.Round(subTotalOrderItemPrice - commissionAmount, 0, MidpointRounding.AwayFromZero);
        if (coupon != null) // If there is a voucher, recalculate the profit
        {
            if (coupon.Type != CouponType.System) // If voucher is system, the discount amount is deducted from system profit
            {
                var discountAmount = subTotalOrderItemPrice * (decimal)(coupon.DiscountPercent / 100) > coupon.MaxDiscount ? coupon.MaxDiscount : subTotalOrderItemPrice * (decimal)(coupon.DiscountPercent / 100);
                commissionAmount = (subTotalOrderItemPrice - discountAmount) * orderItem.Order.CommissionRate;
                merchantPtProfit = subTotalOrderItemPrice - discountAmount - commissionAmount;
            }
        }
        return Math.Round(merchantPtProfit, 0, MidpointRounding.AwayFromZero);
    }

    public async Task<bool> PurchaseGymCourse(long orderCode)
    {
        defaultProfitDistributionDays = await GetProfitDistributionDays();
        autoMarkAsFeedbackAfterDays = await GetAutoMarkAsFeedbackAfterDays();
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
            OrderEntity.Status = OrderStatus.Finished;
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
                var profitDistributionDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(defaultProfitDistributionDays);
                orderItem.ProfitDistributePlannedDate = profitDistributionDate;
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
                if (orderItem.GymPtId != null)
                {
                    await _scheduleJobServices.ScheduleAutoUpdatePTCurrentCourseJob(orderItem.Id, expirationDate);
                }
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
                await _scheduleJobServices.ScheduleAutoMarkAsFeedbackJob(orderItem.Id, profitDistributionDate.AddDays(autoMarkAsFeedbackAfterDays).ToDateTime(TimeOnly.MaxValue));
            }
        }
        await _scheduleJobServices.CancelScheduleJob($"AutoCancelCreatedOrder_{OrderEntity.Id}", "AutoCancelCreatedOrder");
        return true;
    }

    public async Task<bool> ExtendFreelancePTPackage(long orderCode)
    {
        autoMarkAsFeedbackAfterDays = await GetAutoMarkAsFeedbackAfterDays();
        var transactionToExtend = await _unitOfWork.Repository<Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeWithIncludeSpec(orderCode), false);
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

        var numOfSession = orderItemToExtend.FreelancePTPackage.NumOfSessions;
        customerPurchasedToExtend.AvailableSessions += orderItemToExtend.Quantity * numOfSession;
        customerPurchasedToExtend.ExpirationDate = customerPurchasedToExtend.ExpirationDate.AddDays(orderItemToExtend.FreelancePTPackage.DurationInDays * orderItemToExtend.Quantity);

        var profitDistributePlannedDate = customerPurchasedToExtend.ExpirationDate.AddDays(1); // Profit distribute planned date is the day after the expiration date

        orderItemToExtend.ProfitDistributePlannedDate = profitDistributePlannedDate;

        transactionToExtend.Order.Status = OrderStatus.Finished;
        var walletToUpdate = await _unitOfWork.Repository<Wallet>().GetByIdAsync(orderItemToExtend.FreelancePTPackage.PtId);
        if (walletToUpdate == null)
        {
            throw new NotFoundException("Wallet not found");
        }
        var profit = await CalculateMerchantProfit(orderItemToExtend, transactionToExtend.Order.Coupon);
        walletToUpdate.PendingBalance += profit;
        _logger.LogInformation($"Wallet {walletToUpdate.Id} updated with new pending balance {walletToUpdate.PendingBalance} after adding profit {profit}");

        _unitOfWork.Repository<Wallet>().Update(walletToUpdate);
        await _unitOfWork.CommitAsync();
        await _scheduleJobServices.ScheduleProfitDistributionJob(new ProfitJobScheduleDto
        {
            OrderItemId = orderItemToExtend.Id,
            ProfitDistributionDate = profitDistributePlannedDate
        });
        var originalCustomerPurchasedOrderItem = customerPurchasedToExtend.OrderItems.OrderBy(o => o.CreatedAt).First();
        
        await _scheduleJobServices.RescheduleJob($"AutoUpdatePTCurrentCourse_{originalCustomerPurchasedOrderItem.Id}", "AutoUpdatePTCurrentCourse", customerPurchasedToExtend.ExpirationDate.ToDateTime(TimeOnly.MaxValue));

        await _scheduleJobServices.CancelScheduleJob($"AutoCancelCreatedOrder_{transactionToExtend.Order.Id}", "AutoCancelCreatedOrder");

        await _scheduleJobServices.ScheduleAutoMarkAsFeedbackJob(orderItemToExtend.Id, profitDistributePlannedDate.AddDays(autoMarkAsFeedbackAfterDays).ToDateTime(TimeOnly.MinValue));
        return true;
    }

    public async Task<bool> DistributePendingProfit(Guid CustomerPurchasedId)
    {
        var customerPurchased = await _unitOfWork.Repository<CustomerPurchased>().GetByIdAsync(CustomerPurchasedId, false, new List<string> { "OrderItems", "OrderItems.Order", "OrderItems.Order.Coupon", "Bookings", "OrderItems.FreelancePTPackage" });
        if (customerPurchased == null)
        {
            throw new NotFoundException("Customer purchased not found");
        }
        var finishedBookings = customerPurchased.Bookings.Count(b => b.SessionStatus == SessionStatus.Finished);
        var orderItemsList = customerPurchased.OrderItems.OrderBy(o => o.CreatedAt).ToList();
        // Track how many sessions have been "allocated" to previous order items
        var allocatedSessionsForDistribute = 0;
        foreach (var orderItem in orderItemsList)
        {
            var numOfSessionForDistribute = (int)Math.Ceiling(orderItem.Quantity * orderItem.FreelancePTPackage.NumOfSessions / 2.0); //Customer have to finished more than half of the sessions that they have purchased in this order item to distribute profit
            allocatedSessionsForDistribute += numOfSessionForDistribute;

            if (finishedBookings >= allocatedSessionsForDistribute
            && orderItem.ProfitDistributeActualDate == null)
            {
                var distributeDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(1);
                var jobStatus = await _scheduleJobServices.GetJobStatus($"ProfitDistribution_{orderItem.Id}", "ProfitDistribution");
                _logger.LogInformation($"Profit distribution job for order item {orderItem.Id} state is {jobStatus}");
                if (jobStatus == TriggerState.Paused)
                {
                    _logger.LogInformation($"Profit distribution job for order item {orderItem.Id} is already paused");
                    continue;
                }
                if (jobStatus == TriggerState.Normal)
                {
                    _logger.LogInformation($"Profit distribution job state is {jobStatus}");
                    // var rescheduleJob = await _scheduleJobServices.RescheduleJob($"ProfitDistribution_{orderItem.Id}", "ProfitDistribution", distributeDate.ToDateTime(TimeOnly.MinValue)); //Reschedule the job if it is not paused
                    var rescheduleJob = await _scheduleJobServices.RescheduleJob($"ProfitDistribution_{orderItem.Id}", "ProfitDistribution", DateTime.UtcNow);
                    if (!rescheduleJob)
                    {
                        _logger.LogError($"Failed to reschedule profit distribution job for order item {orderItem.Id}");
                        continue;
                    }
                    orderItem.ProfitDistributePlannedDate = distributeDate;
                    orderItem.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<OrderItem>().Update(orderItem);
                    _logger.LogInformation($"Successfully rescheduled profit distribution job for order item {orderItem.Id} at {distributeDate}");

                }
            }
        }
        _logger.LogInformation("Number of finished booking:" + finishedBookings);
        _logger.LogInformation("Number of allocated session:" + allocatedSessionsForDistribute);
        await _unitOfWork.CommitAsync();

        return true;
    }
    public async Task<bool> PurchaseSubscriptionPlans(long orderCode)
    {
        var transactionToPurchaseSubscriptionPlans = await _unitOfWork.Repository<Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeWithIncludeSpec(orderCode), false);
        if (transactionToPurchaseSubscriptionPlans == null)
        {
            throw new NotFoundException("Transaction not found with order code " + orderCode);
        }
        var subscriptionPlansInformation = await _unitOfWork.Repository<SubscriptionPlansInformation>().GetByIdAsync(transactionToPurchaseSubscriptionPlans.Order!.OrderItems.First().SubscriptionPlansInformationId!.Value, includes: new List<string> { "FeatureKey" });
        if (subscriptionPlansInformation == null)
        {
            throw new NotFoundException("Subscription plans information not found");
        }
        int? assignLimitUsage;
        if (subscriptionPlansInformation.FeatureKey.FeatureName == ProjectConstant.FeatureKeyNames.HotResearch)
        {
            assignLimitUsage = null;
            transactionToPurchaseSubscriptionPlans.Order.Account.hotResearch = true;
        }
        else
        {
            assignLimitUsage = subscriptionPlansInformation.LimitUsage;
        }
        transactionToPurchaseSubscriptionPlans.ProfitAmount = transactionToPurchaseSubscriptionPlans.Amount;

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(subscriptionPlansInformation.Duration);

        var newUserSubscription = new UserSubscription
        {
            UserId = transactionToPurchaseSubscriptionPlans.Order!.AccountId,
            SubscriptionPlanId = transactionToPurchaseSubscriptionPlans.Order.OrderItems.First().SubscriptionPlansInformationId!.Value,
            StartDate = startDate,
            EndDate = endDate,
            LimitUsage = assignLimitUsage,
            CurrentUsage = 0,
            Status = SubScriptionStatus.Active,
        };
        _unitOfWork.Repository<Transaction>().Update(transactionToPurchaseSubscriptionPlans);
        _unitOfWork.Repository<UserSubscription>().Insert(newUserSubscription);
        await _unitOfWork.CommitAsync();

        var remindExpiredSubscriptionBeforeDays = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.RemindExpiredSubscriptionBeforeDays);

        await _scheduleJobServices.ScheduleSendRemindExpiredSubscriptionNotiJob(newUserSubscription.Id, endDate.AddDays(-remindExpiredSubscriptionBeforeDays));

        _logger.LogInformation($"Successfully scheduled send remind expired subscription notification job for user subscription {newUserSubscription.Id} at {endDate.AddDays(-remindExpiredSubscriptionBeforeDays)}");

        await _scheduleJobServices.ScheduleExpireUserSubscriptionJob(newUserSubscription.Id, endDate);
        _logger.LogInformation($"Successfully scheduled expire user subscription job for user subscription {newUserSubscription.Id} at {endDate.ToLocalTime}");
        return true;
    }

    public async Task<bool> UpdateOrderShippingDetails(Guid orderId, decimal shippingActualCost, string shippingTrackingId)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId, includes: new List<string> { "Transactions", "OrderStatusHistories" });
        if (order == null)
        {
            throw new NotFoundException($"Order with ID {orderId} not found");
        }

        // Update order shipping actual cost and Ahamove order ID
        // order.ShippingFeeActualCost += shippingActualCost;
        order.ShippingTrackingId = shippingTrackingId;
        var oldStatus = order.Status;
        order.Status = OrderStatus.Assigning;
        var orderStatusHistory = new OrderStatusHistory
        {
            OrderId = orderId,
            Status = OrderStatus.Assigning,
            Description = "Order status updated to Assigning",
            PreviousStatus = oldStatus,
        };
        _unitOfWork.Repository<OrderStatusHistory>().Insert(orderStatusHistory);

        _logger.LogInformation($"Order {orderId} updated with shipping actual cost {shippingActualCost}, Shipping Tracking ID {shippingTrackingId}, and status changed to Assigning");

        // // Calculate profit from shipping fee difference
        // var shippingDifference = shippingActualCost - order.ShippingFee;
        // if(order.OrderStatusHistories.Any(o => o.Status == OrderStatus.Returned))
        // {
        //     shippingDifference = shippingActualCost; // If the order is returned, and the admin send the shipping order again the profit will be minus by the new shipping actual cost
        // }

        // Update transaction profit amount
        // var transaction = order.Transactions.FirstOrDefault();
        // if (transaction != null)
        // {
        //     // Add shipping profit to existing profit amount
        //     var currentProfit = transaction.ProfitAmount ?? 0;
        //     transaction.ProfitAmount = currentProfit - shippingDifference;

        //     _logger.LogInformation($"Transaction for Order {orderId} updated with profit amount {transaction.ProfitAmount} (shipping profit: {shippingDifference})");

        //     _unitOfWork.Repository<Transaction>().Update(transaction);
        // }

        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation($"Successfully updated Order {orderId} with Shipping Tracking ID {shippingTrackingId}");

        return true;
    }
    public async Task<bool> PurchaseAppleSubscriptionPlans(AsnDecodedPayload asnDecodedPayload, JwsTransactionDecoded jwsTransactionDecoded)
    {
        // var orderItemToInsert = new OrderItem
        // {
        //     Quantity = 1,
        //     Price = jwsTransactionDecoded.Price.Value,
        //     SubscriptionPlansInformationId = Guid.Parse(jwsTransactionDecoded.ProductId),
        // };
        // var orderToInsert = new Order
        // {
        //     AccountId = Guid.Parse(jwsTransactionDecoded.AppAppleId.Value.ToString()),
        //     Status = OrderStatus.Finished,
        //     OrderItems = new List<OrderItem> { orderItemToInsert },
        // };
        // var transactionToInsert = new Transaction
        // {
        //     Amount = jwsTransactionDecoded.Price.Value,
        //     OrderCode = long.Parse(jwsTransactionDecoded.TransactionId),
        //     OrderItemId = jwsTransactionDecoded.OrderItemId,
        //     TransactionType = TransactionType.SubscriptionPlansOrder,
        //     Status = TransactionStatus.Success,
        // };
        // if (transactionToPurchaseSubscriptionPlans == null)
        // {
        //     throw new NotFoundException("Transaction not found with order code " + jwsTransactionDecoded.TransactionId);
        // }
        // var subscriptionPlansInformation = await _unitOfWork.Repository<SubscriptionPlansInformation>().GetByIdAsync(transactionToPurchaseSubscriptionPlans.Order!.OrderItems.First().SubscriptionPlansInformationId!.Value, includes: new List<string> { "FeatureKey" });
        // if(subscriptionPlansInformation == null)
        return true;
    }
    public async Task<bool> PurchaseProduct(long orderCode)
    {
        var transactionToPurchaseProduct = await _unitOfWork.Repository<Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeWithIncludeSpec(orderCode), false);
        if (transactionToPurchaseProduct == null)
        {
            throw new NotFoundException("Transaction not found with order code " + orderCode);
        }
        if (transactionToPurchaseProduct.Order.Coupon != null)
        {
            transactionToPurchaseProduct.Order.Coupon.Quantity--;
            transactionToPurchaseProduct.Order.Coupon.NumberOfUsedCoupon++;
        }
        var profit = transactionToPurchaseProduct.Order.TotalAmount;
        foreach (var orderItem in transactionToPurchaseProduct.Order.OrderItems)
        {
            if (orderItem.ProductDetailId != null)
            {
                profit -= orderItem.OriginalProductPrice.Value * orderItem.Quantity;
                // orderItem.ProductDetail.Quantity -= orderItem.Quantity;
                // orderItem.ProductDetail.SoldQuantity += orderItem.Quantity;
            }
        }
        profit -= transactionToPurchaseProduct.Order.ShippingFee;
        var previousStatus = transactionToPurchaseProduct.Order.Status;
        transactionToPurchaseProduct.Order.Status = OrderStatus.Pending;
        transactionToPurchaseProduct.ProfitAmount = profit;
        var orderStatusHistory = new OrderStatusHistory
        {
            OrderId = transactionToPurchaseProduct.OrderId!.Value,
            Status = OrderStatus.Pending,
            Description = "Order status updated to Pending",
            PreviousStatus = previousStatus,
        };
        await _scheduleJobServices.CancelScheduleJob($"AutoCancelCreatedOrder_{transactionToPurchaseProduct.Order.Id}", "AutoCancelCreatedOrder");
        _unitOfWork.Repository<OrderStatusHistory>().Insert(orderStatusHistory);
        await _unitOfWork.CommitAsync();
        return true;
    }
}
