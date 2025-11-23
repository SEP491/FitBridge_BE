using System;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Application.Dtos.OrderItems;
using AutoMapper;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedAvailableByPtId;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseById;
using FitBridge_Application.Specifications.Accounts;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByGymId;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;
using FitBridge_Application.Commons.Constants;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Application.Specifications.UserSubscriptions.GetUserSubscriptionByUserId;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Features.Payments.RePaidOrder;

public class RePaidOrderCommandHandler(IUnitOfWork _unitOfWork, IPayOSService _payOSService, IMapper _mapper, CouponService couponService, IApplicationUserService _applicationUserService, SystemConfigurationService systemConfigurationService, SubscriptionService subscriptionService, OrderService _orderService) : IRequestHandler<RePaidOrderCommand, string>
{
    public async Task<string> Handle(RePaidOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, includes: new List<string> { nameof(Order.Transactions), "Account", "OrderItems" });
        if (order == null)
        {
            throw new NotFoundException("Order not found");
        }
        if (order.Status != OrderStatus.Created)
        {
            throw new BusinessException("Order is not in created status, current status: " + order.Status);
        }
        var transactionToCheck = order.Transactions.FirstOrDefault(x => x.Status == TransactionStatus.Pending);
        if (transactionToCheck == null)
        {
            throw new BusinessException("Order is not pending payment");
        }
        var paymentInfo = await _payOSService.GetPaymentInfoAsync(transactionToCheck.OrderCode.ToString());
        if (paymentInfo.Data.Status == "PENDING" || paymentInfo.Data.Status == "PROCESSING")
        {
            return order.CheckoutUrl;
        }
        transactionToCheck.Status = TransactionStatus.Expired;

        if (paymentInfo.Data.Status == "CANCELLED")
        {
            transactionToCheck.Status = TransactionStatus.Failed;
        }
        transactionToCheck.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Transaction>().Update(transactionToCheck);
        var orderItemDtos = _mapper.Map<List<OrderItemDto>>(order.OrderItems);
        await GetAndValidateOrderItems(orderItemDtos, order.AccountId, order.CouponId, order.CustomerPurchasedIdToExtend);

        var repaidPaymentResponse = await _payOSService.CreatePaymentLinkAsync(new CreatePaymentRequestDto
        {
            AccountId = order.AccountId,
            SubTotalPrice = order.SubTotalPrice,
            TotalAmountPrice = order.TotalAmount,
            OrderItems = orderItemDtos,
            PaymentMethodId = transactionToCheck.PaymentMethodId,
        }, order.Account);
        order.CheckoutUrl = repaidPaymentResponse.Data.CheckoutUrl;
        await CreateNewTransaction(order, repaidPaymentResponse, transactionToCheck);
        order.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.CommitAsync();
        return repaidPaymentResponse.Data.CheckoutUrl;
    }
    public async Task CreateNewTransaction(Order order, PaymentResponseDto paymentResponse, Transaction transactionToCheck)
    {
        var newTransaction = new Transaction
        {
            OrderCode = paymentResponse.Data.OrderCode,
            Description = "Repayment for order " + order.Id,
            PaymentMethodId = transactionToCheck.PaymentMethodId,
            TransactionType = transactionToCheck.TransactionType,
            Status = TransactionStatus.Pending,
            OrderId = order.Id,
            Amount = paymentResponse.Data.Amount,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        _unitOfWork.Repository<Transaction>().Insert(newTransaction);
    }
    
     public async Task GetAndValidateOrderItems(List<OrderItemDto> OrderItems, Guid userId, Guid? couponId, Guid? customerPurchasedIdToExtend)
    {
        if (couponId != null)
        {
            await couponService.ApplyCouponAsync(userId, couponId.Value, OrderItems.Count);
        }

        foreach (var item in OrderItems)
        {
            if (item.GymCourseId != null)
            {
                var gymCoursePT = await _unitOfWork.Repository<GymCourse>().GetBySpecificationAsync(new GetGymCourseByIdSpecification(item.GymCourseId.Value));

                if (gymCoursePT == null)
                {
                    throw new NotFoundException("Gym course PT not found");
                }
                item.ProductName = gymCoursePT.Name;

                if (item.GymPtId != null)
                {
                    var gymPt = await _applicationUserService.GetUserWithSpecAsync(new GetAccountByIdSpecificationForUserProfile(item.GymPtId.Value));
                    if (gymPt == null)
                    {
                        throw new NotFoundException("Gym PT not found");
                    }
                    var currentCourseCount = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(new GetCustomerPurchasedAvailableByPtIdSpec(item.GymPtId.Value, null));
                    if (currentCourseCount >= gymPt.PtMaxCourse)
                    {
                        throw new BusinessException($"Maximum course count reached for PT {gymPt.FullName}, current course count: {currentCourseCount}, maximum course count: {gymPt.PtMaxCourse}");
                    }
                }

                var userPackage = await _unitOfWork.Repository<CustomerPurchased>().GetBySpecificationAsync(new GetCustomerPurchasedByGymIdSpec(gymCoursePT.GymOwnerId, userId));
                if (userPackage != null && customerPurchasedIdToExtend == null)
                {
                    throw new PackageExistException($"Package of this gym still not expired, customer purchased id: {userPackage.Id}, package expiration date: {userPackage.ExpirationDate} please extend the package");
                }
            }

            if (item.FreelancePTPackageId != null)
            {
                var freelancePTPackage = await _unitOfWork.Repository<FreelancePTPackage>().GetBySpecificationAsync(new GetFreelancePtPackageByIdSpec(item.FreelancePTPackageId.Value));
                if (freelancePTPackage == null)
                {
                    throw new NotFoundException("Freelance PTPackage not found");
                }
                item.ProductName = freelancePTPackage.Name;
                var userPackage = await _unitOfWork.Repository<CustomerPurchased>().GetBySpecificationAsync(new GetCustomerPurchasedByFreelancePtIdSpec(freelancePTPackage.PtId, userId));
                if (userPackage != null && customerPurchasedIdToExtend == null)
                {
                    throw new PackageExistException($"Package of this freelance PT still not expired, customer purchased id: {userPackage.Id}, package expiration date: {userPackage.ExpirationDate} please extend the package");
                }
                var freelancePt = await _applicationUserService.GetByIdAsync(freelancePTPackage.PtId);
                if (freelancePt == null)
                {
                    throw new NotFoundException("Freelance PT not found");
                }
                var currentCourseCount = await _unitOfWork.Repository<CustomerPurchased>().CountAsync(new GetCustomerPurchasedAvailableByPtIdSpec(null, freelancePTPackage.PtId));
                if (currentCourseCount >= freelancePt.PtMaxCourse)
                {
                    throw new BusinessException($"Maximum course count reached for freelance PT {freelancePt.FullName}, current course count: {currentCourseCount}, maximum course count: {freelancePt.PtMaxCourse}");
                }
            }
            if (item.SubscriptionPlansInformationId != null)
            {
                var maxHotResearchSubscription = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.HotResearchSubscriptionLimit);

                var subscriptionPlansInformation = await _unitOfWork.Repository<SubscriptionPlansInformation>().GetByIdAsync(item.SubscriptionPlansInformationId.Value, includes: new List<string> { "FeatureKey" });
                if (subscriptionPlansInformation == null)
                {
                    throw new NotFoundException("Subscription plans information not found");
                }
                if (subscriptionPlansInformation.FeatureKey.FeatureName == ProjectConstant.FeatureKeyNames.HotResearch)
                {
                    var numOfCurrentHotResearchSubscription = await subscriptionService.GetNumOfCurrentHotResearchSubscription();
                    if (numOfCurrentHotResearchSubscription >= maxHotResearchSubscription)
                    {
                        throw new BusinessException("Maximum hot research subscription reached");
                    }
                }

                var userSubscription = await _unitOfWork.Repository<UserSubscription>().GetBySpecificationAsync(new GetUserSubscriptionByUserIdSpec(userId, subscriptionPlansInformation.Id));
                if (userSubscription != null)
                {
                    throw new DuplicateException($"User already has a subscription for this plan, subscription id: {userSubscription.Id}, subscription expiration date: {userSubscription.EndDate}");
                }
            }
            if (item.ProductDetailId != null)
            {
                var productDetail = await _unitOfWork.Repository<ProductDetail>().GetByIdAsync(item.ProductDetailId.Value);
                if (productDetail == null)
                {
                    throw new NotFoundException("Product detail not found");
                }
                if (productDetail.Quantity < item.Quantity)
                {
                    throw new BusinessException("Product quantity is not enough");
                }
                await _orderService.UpdateProductDetailQuantity(productDetail, item.Quantity);
            }
        }
    }
}
