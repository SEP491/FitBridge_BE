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

namespace FitBridge_Application.Features.Payments.CreatePaymentLink;

public class CreatePaymentLinkCommandHandler(IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, IPayOSService _payOSService, IApplicationUserService _applicationUserService, IMapper _mapper, CouponService couponService) : IRequestHandler<CreatePaymentLinkCommand, PaymentResponseDto>
{
    public async Task<PaymentResponseDto> Handle(CreatePaymentLinkCommand request, CancellationToken cancellationToken)
    {
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
        await GetAndValidateOrderItems(request.Request.OrderItems, userId.Value, request.Request.CouponId, request.Request.CustomerPurchasedIdToExtend);
        var SubTotalPrice = CalculateSubTotalPrice(request.Request.OrderItems);
        request.Request.SubTotalPrice = SubTotalPrice;
        request.Request.AccountId = userId;
        var calculateTotalPrice = await CalculateTotalPrice(request.Request, userId.Value);
        request.Request.TotalAmountPrice = calculateTotalPrice;

        var paymentResponse = await _payOSService.CreatePaymentLinkAsync(request.Request, user);
        var orderId = await CreateOrder(request.Request, paymentResponse.Data.CheckoutUrl, userId.Value);
        await CreateTransaction(paymentResponse, request, orderId);
        await AssignOrderItemProductName(request.Request.OrderItems);
        await _unitOfWork.CommitAsync();

        return paymentResponse;
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
        if(request.Request.OrderItems.Any(x => x.FreelancePTPackageId != null) && request.Request.CustomerPurchasedIdToExtend != null)
        {
            transactionType = TransactionType.ExtendFreelancePTPackage;
        }
        if(request.Request.OrderItems.Any(x => x.GymCourseId != null) && request.Request.CustomerPurchasedIdToExtend != null)
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
            Amount = paymentResponse.Data.Amount
        };
        _unitOfWork.Repository<Transaction>().Insert(newTransaction);
    }

    public async Task<Guid> CreateOrder(CreatePaymentRequestDto request, string checkoutUrl, Guid userId)
    {
        var order = _mapper.Map<Order>(request);
        order.SubTotalPrice = request.SubTotalPrice;
        order.TotalAmount = request.TotalAmountPrice;
        order.Status = OrderStatus.Created;
        order.CheckoutUrl = checkoutUrl;
        order.CouponId = request.CouponId ?? null;
        order.CustomerPurchasedIdToExtend = request.CustomerPurchasedIdToExtend ?? null;
        _unitOfWork.Repository<Order>().Insert(order);
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

    public async Task GetAndValidateOrderItems(List<OrderItemDto> OrderItems, Guid userId, Guid? couponId, Guid? customerPurchasedIdToExtend)
    {
        if (couponId != null)
        {
            var coupon = await _unitOfWork.Repository<Coupon>().GetByIdAsync(couponId.Value);
            if (coupon == null)
            {
                throw new NotFoundException("Coupon not found");
            }
            if(coupon.Type != CouponType.System && OrderItems.Count > 1)
            {
                throw new NotFoundException("This coupon type only can be used for system or gym owner");
            }
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

                if (item.GymPtId != null)
                {
                    var gymPt = await _applicationUserService.GetUserWithSpecAsync(new GetAccountByIdSpecificationForUserProfile(item.GymPtId.Value));
                    if (gymPt == null)
                    {
                        throw new NotFoundException("Gym PT not found");
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
            var priceAfterDiscount = await couponService.ApplyCouponAsync(userId, request.CouponId.Value, request.SubTotalPrice);
            return priceAfterDiscount.DiscountAmount + request.ShippingFee;
        }

        return request.SubTotalPrice + request.ShippingFee;
    }
}