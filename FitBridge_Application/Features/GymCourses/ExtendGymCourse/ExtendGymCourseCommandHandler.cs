using AutoMapper;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications.Accounts;
using FitBridge_Application.Specifications.Coupons;
using FitBridge_Application.Specifications.Coupons.GetCouponById;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedById;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseById;
using FitBridge_Application.Specifications.ProductDetails;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;

namespace FitBridge_Application.Features.GymCourses.ExtendGymCourse;

public class ExtendGymCourseCommandHandler(IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService, 
    IPayOSService _payOSService, IMapper _mapper, CouponService couponService) : IRequestHandler<ExtendGymCourseCommand, PaymentResponseDto>
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

        var requestDto = new CreatePaymentRequestDto();
        requestDto.CustomerPurchasedIdToExtend = request.CustomerPurchasedIdToExtend;
        requestDto.PaymentMethodId = request.PaymentMethodId;
        requestDto.OrderItems =
        [
            new OrderItemDto() {
                Quantity = request.Quantity,
            },
        ];
        await GetAndValidateOrderItems(requestDto.OrderItems, request.CustomerPurchasedIdToExtend);

        var subTotalPrice = CalculateSubTotalPrice(requestDto.OrderItems);
        requestDto.SubTotalPrice = subTotalPrice;
        var totalPrice = await CalculateTotalPrice(requestDto, userId.Value);
        requestDto.TotalAmountPrice = totalPrice;
        requestDto.AccountId = userId;

        var paymentResponse = await _payOSService.CreatePaymentLinkAsync(requestDto, user);
        var orderId = await CreateOrder(requestDto, paymentResponse.Data.CheckoutUrl, userId.Value);
        await CreateTransaction(paymentResponse, requestDto.PaymentMethodId, orderId);
        await AssignOrderItemProductName(requestDto.OrderItems);

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

    public async Task<Guid> CreateOrder(CreatePaymentRequestDto request, string checkoutUrl, Guid userId)
    {
        var order = _mapper.Map<Order>(request);
        order.SubTotalPrice = request.SubTotalPrice;
        order.TotalAmount = request.TotalAmountPrice;
        order.Status = OrderStatus.Created;
        order.CheckoutUrl = checkoutUrl;
        order.CouponId = request.CouponId ?? null;
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

    public decimal CalculateSubTotalPrice(List<OrderItemDto> OrderItems)
    {
        decimal SubTotalPrice = 0;
        foreach (var item in OrderItems)
        {
            SubTotalPrice += item.Price * item.Quantity;
        }
        return SubTotalPrice;
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