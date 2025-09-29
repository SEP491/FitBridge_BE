using System;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using MediatR;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseById;
using FitBridge_Application.Specifications.Accounts;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByGymId;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedById;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;
using FitBridge_Application.Specifications.ProductDetails;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Application.Specifications.Coupons;

namespace FitBridge_Application.Features.GymCourses.ExtendGymCourse;

public class ExtendGymCourseCommandHandler(IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService, IPayOSService _payOSService, IMapper _mapper) : IRequestHandler<ExtendGymCourseCommand, PaymentResponseDto>
{
    public async Task<PaymentResponseDto> Handle(ExtendGymCourseCommand request, CancellationToken cancellationToken)
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
        if (request.Request.CustomerPurchasedIdToExtend == null)
        {
            throw new DataValidationFailedException("Customer purchased to extend id cannot be null");
        }
        await GetAndValidateOrderItems(request.Request.OrderItems, request.Request.CustomerPurchasedIdToExtend.Value);

        var totalPrice = CalculateTotalPrice(request.Request.OrderItems);
        request.Request.TotalAmount = totalPrice;
        request.Request.AccountId = userId;

        var paymentResponse = await _payOSService.CreatePaymentLinkAsync(request.Request, user);
        var orderId = await CreateOrder(request.Request, paymentResponse.Data.CheckoutUrl);
        await CreateTransaction(paymentResponse, request.Request.PaymentMethodId, orderId);
        await AssignOrderItemProductName(request.Request.OrderItems);

        return paymentResponse;
    }

    public async Task CreateTransaction(PaymentResponseDto paymentResponse, Guid paymentMethodId, Guid orderId)
    {
        var newTransaction = new Transaction
        {
            OrderCode = paymentResponse.Data.OrderCode,
            Description = "Payment for order " + paymentResponse.Data.OrderCode,
            PaymentMethodId = paymentMethodId,
            TransactionType = TransactionType.ExtendCourse,
            Status = TransactionStatus.Pending,
            OrderId = orderId,
            Amount = paymentResponse.Data.Amount
        };
        _unitOfWork.Repository<Transaction>().Insert(newTransaction);
        await _unitOfWork.CommitAsync();
    }

    public async Task<Guid> CreateOrder(CreatePaymentRequestDto request, string checkoutUrl)
    {
        var subTotalPrice = await CalculateSubTotalPrice(request);
        var order = _mapper.Map<Order>(request);
        order.SubTotalPrice = subTotalPrice;
        order.Status = OrderStatus.PaymentProcessing;
        order.CheckoutUrl = checkoutUrl;
        _unitOfWork.Repository<Order>().Insert(order);
        await _unitOfWork.CommitAsync();
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
            if (item.ServiceInformationId != null)
            {
                var serviceInformation = await _unitOfWork.Repository<ServiceInformation>().GetByIdAsync(item.ServiceInformationId.Value);
                if (serviceInformation == null)
                {
                    throw new NotFoundException("Service information not found");
                }
                item.ProductName = serviceInformation.ServiceName;
            }
        }
    }

    public async Task GetAndValidateOrderItems(List<OrderItemDto> OrderItems, Guid customerPurchasedIdToExtend)
    {
        foreach (var item in OrderItems)
        {
            var userPackage = await _unitOfWork.Repository<CustomerPurchased>().GetBySpecificationAsync(new GetCustomerPurchasedByIdSpec(customerPurchasedIdToExtend));

            if (userPackage == null)
            {
                throw new NotFoundException("Can't find customer purchased to extend");
            }
            var orderItemToExtend = userPackage.OrderItems.OrderByDescending(x => x.CreatedAt).First();

            if (orderItemToExtend.GymCourseId != null)
            {
                var gymCourse = await _unitOfWork.Repository<GymCourse>().GetBySpecificationAsync(new GetGymCourseByIdSpecification(orderItemToExtend.GymCourseId.Value));

                if (gymCourse == null)
                {
                    throw new NotFoundException("Gym course PT not found");
                }

                if (orderItemToExtend.GymPtId != null)
                {
                    var gymPt = await _applicationUserService.GetUserWithSpecAsync(new GetAccountByIdSpecificationForUserProfile(orderItemToExtend.GymPtId.Value));
                    if (gymPt == null)
                    {
                        throw new NotFoundException("Gym PT not found");
                    }
                    item.GymPtId = orderItemToExtend.GymPtId.Value;
                    item.Price = gymCourse.Price + gymCourse.PtPrice;
                }
                else
                {
                    item.Price = gymCourse.Price;
                }
                item.GymCourseId = orderItemToExtend.GymCourseId.Value;
            }

            if (orderItemToExtend.FreelancePTPackageId != null)
            {
                var freelancePTPackage = await _unitOfWork.Repository<FreelancePTPackage>().GetBySpecificationAsync(new GetFreelancePtPackageByIdSpec(orderItemToExtend.FreelancePTPackageId.Value));
                if (freelancePTPackage == null)
                {
                    throw new NotFoundException("Freelance PTPackage not found");
                }
                item.FreelancePTPackageId = orderItemToExtend.FreelancePTPackageId.Value;
                item.Price = freelancePTPackage.Price;
            }
        }
    }

    public decimal CalculateTotalPrice(List<OrderItemDto> OrderItems)
    {
        decimal totalPrice = 0;
        foreach (var item in OrderItems)
        {
            totalPrice += item.Price * item.Quantity;
        }
        return totalPrice;
    }

    public async Task<decimal> CalculateSubTotalPrice(CreatePaymentRequestDto request)
    {
        if (request.CouponId != null)
        {
            var coupon = await _unitOfWork.Repository<Coupon>().GetBySpecificationAsync(new GetCouponByIdSpec(request.CouponId!.Value));
            if (coupon == null)
            {
                throw new NotFoundException("Coupon not found");
            }
            var couponDiscountAmount = (decimal)coupon.DiscountPercent / 100 * request.TotalAmount > coupon.MaxDiscount ? coupon.MaxDiscount : (decimal)coupon.DiscountPercent / 100 * request.TotalAmount;
            return request.TotalAmount - couponDiscountAmount;
        }

        return request.TotalAmount;
    }
}