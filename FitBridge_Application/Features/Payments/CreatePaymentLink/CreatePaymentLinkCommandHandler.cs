using System;
using FitBridge_Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using MediatR;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Application.Specifications.ProductDetails;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseById;
using FitBridge_Application.Specifications.Accounts;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByGymId;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;
using FitBridge_Domain.Entities.Orders;
using AutoMapper;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Application.Specifications.Coupons;
using FitBridge_Application.Specifications.Coupons.GetCouponById;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications.UserSubscriptions.GetUserSubscriptionByUserId;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedAvailableByPtId;
using FitBridge_Domain.Enums.SubscriptionPlans;

namespace FitBridge_Application.Features.Payments.CreatePaymentLink;

public class CreatePaymentLinkCommandHandler(IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, IPayOSService _payOSService, IApplicationUserService _applicationUserService, IMapper _mapper, CouponService couponService, SubscriptionService subscriptionService, SystemConfigurationService systemConfigurationService, ITransactionService _transactionService, OrderService _orderService, IScheduleJobServices _scheduleJobServices) : IRequestHandler<CreatePaymentLinkCommand, PaymentResponseDto>
{
    public async Task<PaymentResponseDto> Handle(CreatePaymentLinkCommand request, CancellationToken cancellationToken)
    {
        var paymentMethod = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(request.Request.PaymentMethodId);
        if (paymentMethod == null)
        {
            throw new NotFoundException("Payment method not found");
        }
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User Id not found");
        }
        var user = await _applicationUserService.GetByIdAsync(userId.Value);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        await GetAndValidateOrderItems(request.Request.OrderItems, userId.Value, request.Request.CustomerPurchasedIdToExtend);
        var SubTotalPrice = CalculateSubTotalPrice(request.Request.OrderItems);
        request.Request.SubTotalPrice = SubTotalPrice;
        request.Request.AccountId = userId;
        var calculateTotalPrice = await CalculateTotalPrice(request.Request, userId.Value);
        request.Request.TotalAmountPrice = calculateTotalPrice;
        var paymentResponse = new PaymentResponseDto();
        if (paymentMethod.MethodType == MethodType.COD)
        {
            await CreateCodOrder(request.Request, userId.Value);
            paymentResponse.IsCOD = true;
        }
        else
        {
            paymentResponse = await _payOSService.CreatePaymentLinkAsync(request.Request, user);
            var orderId = await CreateOrder(request.Request, paymentResponse.Data.CheckoutUrl, userId.Value, OrderStatus.Created);
            await CreateTransaction(paymentResponse, request, orderId);
            await AssignOrderItemProductName(request.Request.OrderItems);
            await _unitOfWork.CommitAsync();
        }

        return paymentResponse;
    }

    public async Task<bool> CreateCodOrder(CreatePaymentRequestDto request, Guid userId)
    {
        var orderId = await CreateOrder(request, "", userId, OrderStatus.Pending);
        var newTransaction = new Transaction
        {
            OrderCode = _payOSService.GenerateOrderCode(),
            Description = "Payment for order " + orderId,
            PaymentMethodId = request.PaymentMethodId,
            TransactionType = TransactionType.ProductOrder,
            Status = TransactionStatus.Pending,
            OrderId = orderId,
            Amount = request.TotalAmountPrice,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        var createdOrderHistory = new OrderStatusHistory
        {
            OrderId = orderId,
            Status = OrderStatus.Created,
            Description = "Order created",
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        _unitOfWork.Repository<OrderStatusHistory>().Insert(createdOrderHistory);
        _unitOfWork.Repository<Transaction>().Insert(newTransaction);
        await _unitOfWork.CommitAsync();
        await _transactionService.PurchaseProduct(newTransaction.OrderCode);
        return true;
    }
    public async Task CreateTransaction(PaymentResponseDto paymentResponse, CreatePaymentLinkCommand request, Guid orderId)
    {
        var transactionType = TransactionType.ProductOrder;
        if (request.Request.OrderItems.Any(x => x.FreelancePTPackageId != null))
        {
            transactionType = TransactionType.FreelancePTPackage;
        }
        if (request.Request.OrderItems.Any(x => x.GymCourseId != null))
        {
            transactionType = TransactionType.GymCourse;
        }
        if (request.Request.OrderItems.Any(x => x.SubscriptionPlansInformationId != null))
        {
            transactionType = TransactionType.SubscriptionPlansOrder;
        }
        if (request.Request.OrderItems.Any(x => x.FreelancePTPackageId != null) && request.Request.CustomerPurchasedIdToExtend != null)
        {
            transactionType = TransactionType.ExtendFreelancePTPackage;
        }
        if (request.Request.OrderItems.Any(x => x.GymCourseId != null) && request.Request.CustomerPurchasedIdToExtend != null)
        {
            transactionType = TransactionType.ExtendCourse;
        }

        var newTransaction = new Transaction
        {
            OrderCode = paymentResponse.Data.OrderCode,
            Description = "Payment for order " + paymentResponse.Data.OrderCode,
            PaymentMethodId = request.Request.PaymentMethodId,
            TransactionType = transactionType,
            Status = TransactionStatus.Pending,
            OrderId = orderId,
            Amount = paymentResponse.Data.Amount,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        if (transactionType == TransactionType.ProductOrder)
        {
            var newOrderHistory = new OrderStatusHistory
            {
                OrderId = orderId,
                Status = OrderStatus.Created,
                Description = "Order created",
            };
            _unitOfWork.Repository<OrderStatusHistory>().Insert(newOrderHistory);
        }
        _unitOfWork.Repository<Transaction>().Insert(newTransaction);
    }

    public async Task<Guid> CreateOrder(CreatePaymentRequestDto request, string checkoutUrl, Guid userId, OrderStatus status)
    {

        var commissionRate = await GetCurrentCommissionRate();
        if (request.OrderItems.Any(oi => oi.ProductDetailId != null))
        {
            commissionRate = 0;
        }

        if (request.OrderItems.Any(oi => oi.SubscriptionPlansInformationId != null))
        {
            var tempSubscription = new UserSubscription
            {
                UserId = userId,
                SubscriptionPlanId = request.OrderItems.First().SubscriptionPlansInformationId.Value,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                Status = SubScriptionStatus.Created,
            };
            _unitOfWork.Repository<UserSubscription>().Insert(tempSubscription);
            request.OrderItems.First().UserSubscriptionId = tempSubscription.Id;
        }

        var order = _mapper.Map<Order>(request);
        order.SubTotalPrice = request.SubTotalPrice;
        order.TotalAmount = request.TotalAmountPrice;
        order.Status = status;
        order.CheckoutUrl = checkoutUrl;
        order.CouponId = request.CouponId ?? null;
        order.CustomerPurchasedIdToExtend = request.CustomerPurchasedIdToExtend ?? null;
        order.UpdatedAt = DateTime.UtcNow;
        order.CreatedAt = DateTime.UtcNow;
        order.CommissionRate = commissionRate;
        
        _unitOfWork.Repository<Order>().Insert(order);
        await _scheduleJobServices.ScheduleAutoCancelCreatedOrderJob(order.Id);
        return order.Id;
    }

    public async Task AssignOrderItemProductName(List<OrderItemDto> OrderItems)
    {
        foreach (var item in OrderItems)
        {
            if (item.ProductDetailId != null)
            {
                var specification = new GetProductDetailsByIdSpecification(item.ProductDetailId.Value);
                var productDetail = await _unitOfWork.Repository<ProductDetail>().GetBySpecificationAsync(specification);
                if (productDetail == null)
                {
                    throw new NotFoundException("Product detail not found");
                }
                item.ProductName = productDetail.Product.Name;
            }
            if (item.GymCourseId != null)
            {
                var gymCourse = await _unitOfWork.Repository<GymCourse>().GetByIdAsync(item.GymCourseId.Value);
                if (gymCourse == null)
                {
                    throw new NotFoundException("Gym course not found");
                }
                item.ProductName = gymCourse.Name;
            }
            if (item.SubscriptionPlansInformationId != null)
            {
                var subscriptionPlansInformation = await _unitOfWork.Repository<SubscriptionPlansInformation>().GetByIdAsync(item.SubscriptionPlansInformationId.Value);
                if (subscriptionPlansInformation == null)
                {
                    throw new NotFoundException("Subscription plans information not found");
                }
                item.ProductName = subscriptionPlansInformation.PlanName;
            }
            if (item.FreelancePTPackageId != null)
            {
                var freelancePTPackage = await _unitOfWork.Repository<FreelancePTPackage>().GetByIdAsync(item.FreelancePTPackageId.Value);
                if (freelancePTPackage == null)
                {
                    throw new NotFoundException("Freelance PTPackage not found");
                }
                item.ProductName = freelancePTPackage.Name;
            }
        }
    }

    public async Task<decimal> GetCurrentCommissionRate()
    {
        var currentCommissionRate = (decimal)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(
        ProjectConstant.SystemConfigurationKeys.CommissionRate);
        return currentCommissionRate;
    }
    public async Task GetAndValidateOrderItems(List<OrderItemDto> OrderItems, Guid userId, Guid? customerPurchasedIdToExtend)
    {
        foreach (var item in OrderItems)
        {
            if (item.GymCourseId != null)
            {
                var gymCoursePT = await _unitOfWork.Repository<GymCourse>().GetBySpecificationAsync(new GetGymCourseByIdSpecification(item.GymCourseId.Value));

                if (gymCoursePT == null)
                {
                    throw new NotFoundException("Gym course PT not found");
                }

                if (item.GymPtId != null)
                {
                    var gymPt = await _applicationUserService.GetUserWithSpecAsync(new GetAccountByIdSpecificationForUserProfile(item.GymPtId.Value), false);
                    if (gymPt == null)
                    {
                        throw new NotFoundException("Gym PT not found");
                    }
                    if (customerPurchasedIdToExtend == null)
                    {
                        var currentCourseCount = gymPt.PtCurrentCourse;
                        if (currentCourseCount >= gymPt.PtMaxCourse)
                        {
                            throw new BusinessException($"Maximum course count reached for PT {gymPt.FullName}, current course count: {currentCourseCount}, maximum course count: {gymPt.PtMaxCourse}");
                        }
                        gymPt.PtCurrentCourse++;
                        gymPt.UpdatedAt = DateTime.UtcNow;
                    }
                    item.Price = gymCoursePT.Price + gymCoursePT.PtPrice;
                        
                }
                else
                {
                    item.Price = gymCoursePT.Price;
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
                item.Price = freelancePTPackage.Price;
                var userPackage = await _unitOfWork.Repository<CustomerPurchased>().GetBySpecificationAsync(new GetCustomerPurchasedByFreelancePtIdSpec(freelancePTPackage.PtId, userId));
                if (userPackage != null && customerPurchasedIdToExtend == null)
                {
                    throw new PackageExistException($"Package of this freelance PT still not expired, customer purchased id: {userPackage.Id}, package expiration date: {userPackage.ExpirationDate} please extend the package");
                }
                var freelancePt = await _applicationUserService.GetByIdAsync(freelancePTPackage.PtId, null, true);
                if (freelancePt == null)
                {
                    throw new NotFoundException("Freelance PT not found");
                }
                if(customerPurchasedIdToExtend == null) {
                    var currentCourseCount = freelancePt.PtCurrentCourse;
                    if (currentCourseCount >= freelancePt.PtMaxCourse)
                    {
                        throw new BusinessException($"Maximum course count reached for freelance PT {freelancePt.FullName}, current course count: {currentCourseCount}, maximum course count: {freelancePt.PtMaxCourse}");
                    }
                    if(customerPurchasedIdToExtend == null)
                    {
                        freelancePt.PtCurrentCourse++;
                        freelancePt.UpdatedAt = DateTime.UtcNow;
                    }
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
                item.Price = subscriptionPlansInformation.PlanCharge;

                var userSubscription = await _unitOfWork.Repository<UserSubscription>().GetBySpecificationAsync(new GetUserSubscriptionByUserIdSpec(userId, subscriptionPlansInformation.Id));
                if (userSubscription != null)
                {
                    if(userSubscription.Status == SubScriptionStatus.Created)
                    {
                        throw new DuplicateException($"User already has a created subscription for this plan, please finish the payment to activate the subscription");
                    }
                    throw new DuplicateException($"User already has a subscription for this plan, subscription id: {userSubscription.Id}, subscription expiration date: {userSubscription.EndDate}");
                }
            }
            if(item.ProductDetailId != null)
            {
                var productDetail = await _unitOfWork.Repository<ProductDetail>().GetByIdAsync(item.ProductDetailId.Value);
                if (productDetail == null)
                {
                    throw new NotFoundException("Product detail not found");
                }
                if(productDetail.Quantity < item.Quantity)
                {
                    throw new BusinessException("Product quantity is not enough");
                }
                await _orderService.UpdateProductDetailQuantity(productDetail, item.Quantity);
                item.Price = productDetail.SalePrice;
                item.OriginalProductPrice = productDetail.OriginalPrice;
            }
        }
    }

    public decimal CalculateSubTotalPrice(List<OrderItemDto> OrderItems)
    {
        decimal subTotalPrice = 0;
        foreach (var item in OrderItems)
        {
            subTotalPrice += item.Price * item.Quantity;
        }
        return subTotalPrice;
    }

    public async Task<decimal> CalculateTotalPrice(CreatePaymentRequestDto request, Guid userId)
    {
        if (request.CouponId != null)
        {
            var itemsIds = new List<Guid>();
            var productType = string.Empty;
            if (request.OrderItems.Any(x => x.ProductDetailId != null))
            {
                itemsIds = request.OrderItems.Where(x => x.ProductDetailId != null).Select(x => x.ProductDetailId!.Value).ToList();
                productType = nameof(Product);
            }
            else if (request.OrderItems.Any(x => x.GymCourseId != null))
            {
                itemsIds = request.OrderItems.Where(x => x.GymCourseId != null).Select(x => x.GymCourseId!.Value).ToList();
                productType = nameof(GymCourse);
            }
            else if (request.OrderItems.Any(x => x.FreelancePTPackageId != null))
            {
                itemsIds = request.OrderItems.Where(x => x.FreelancePTPackageId != null).Select(x => x.FreelancePTPackageId!.Value).ToList();
                productType = nameof(FreelancePTPackage);
            }
            else if (request.OrderItems.Any(x => x.SubscriptionPlansInformationId != null))
            {
                itemsIds = request.OrderItems.Where(x => x.SubscriptionPlansInformationId != null).Select(x => x.SubscriptionPlansInformationId!.Value).ToList();
                productType = nameof(SubscriptionPlansInformation);
            }
            var coupon = await _unitOfWork.Repository<Coupon>().GetByIdAsync(request.CouponId.Value);
            if (coupon == null)
            {
                throw new NotFoundException("Coupon not found");
            }
            var priceAfterDiscount = await couponService.ApplyCouponWithValidationAsync(coupon.CouponCode,
            userId,
            itemsIds,
            productType,
            request.SubTotalPrice);
            return priceAfterDiscount.DiscountAmount + request.ShippingFee;
        }

        return request.SubTotalPrice + request.ShippingFee;
    }
}