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

namespace FitBridge_Application.Features.Payments.CreatePaymentLink;

public class CreatePaymentLinkCommandHandler(IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, IPayOSService _payOSService, IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<CreatePaymentLinkCommand, PaymentResponseDto>
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
        await GetAndValidateOrderItems(request.Request.OrderItems, userId.Value);
        var totalPrice = CalculateTotalPrice(request.Request.OrderItems);
        request.Request.TotalAmount = totalPrice;
        request.Request.AccountId = userId;

        var paymentResponse = await _payOSService.CreatePaymentLinkAsync(request.Request, user);
        var orderId = await CreateOrder(request.Request, paymentResponse.Data.CheckoutUrl);
        await CreateTransaction(paymentResponse, request.Request.PaymentMethodId, orderId);
        await AssignOrderItemProductName(request.Request.OrderItems);
        await _unitOfWork.CommitAsync();

        return paymentResponse;
    }

    public async Task CreateTransaction(PaymentResponseDto paymentResponse, Guid paymentMethodId, Guid orderId)
    {
        var newTransaction = new Transaction
        {
            OrderCode = paymentResponse.Data.OrderCode,
            Description = "Payment for order " + paymentResponse.Data.OrderCode,
            PaymentMethodId = paymentMethodId,
            TransactionType = TransactionType.ProductOrder,
            Status = TransactionStatus.Pending,
            OrderId = orderId,
            Amount = paymentResponse.Data.Amount
        };
        _unitOfWork.Repository<Transaction>().Insert(newTransaction);
    }

    public async Task<Guid> CreateOrder(CreatePaymentRequestDto request, string checkoutUrl)
    {
        var subTotalPrice = await CalculateSubTotalPrice(request);
        var order = _mapper.Map<Order>(request);
        order.SubTotalPrice = subTotalPrice;
        order.Status = OrderStatus.PaymentProcessing;
        order.CheckoutUrl = checkoutUrl;
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

    public async Task GetAndValidateOrderItems(List<OrderItemDto> OrderItems, Guid userId)
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
                if (userPackage != null)
                {
                    throw new PackageExistException("Package of this gym course still not expired");
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
                if (userPackage != null)
                {
                    throw new PackageExistException("Package of this freelance PT still not expired");
                }
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
            return request.TotalAmount - couponDiscountAmount + request.ShippingFee;
        }

        return request.TotalAmount + request.ShippingFee;
    }
}