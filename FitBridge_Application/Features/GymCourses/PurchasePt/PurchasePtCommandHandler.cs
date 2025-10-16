using System;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MediatR;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedById;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseById;
using FitBridge_Application.Specifications.Accounts;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Features.GymCourses.PurchasePt;

public class PurchasePtCommandHandler(IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService, IPayOSService _payOSService, IMapper _mapper) : IRequestHandler<PurchasePtCommand, PaymentResponseDto>
{
public async Task<PaymentResponseDto> Handle(PurchasePtCommand request, CancellationToken cancellationToken)
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
        var createPaymentRequestDto = new CreatePaymentRequestDto();
        createPaymentRequestDto.OrderItems = new List<OrderItemDto>();
        createPaymentRequestDto.AccountId = userId;
        createPaymentRequestDto.CustomerPurchasedIdToExtend = request.CustomerPurchasedId;
        createPaymentRequestDto.PaymentMethodId = request.PaymentMethodId;
        createPaymentRequestDto.OrderItems.Add(new OrderItemDto()
        {
            Quantity = 1,
        });
        await GetAndValidateOrderItems(createPaymentRequestDto.OrderItems.First(), request.CustomerPurchasedId);
        
        var subTotalPrice = CalculateSubTotalPrice(createPaymentRequestDto.OrderItems.First());
        createPaymentRequestDto.SubTotalPrice = subTotalPrice;
        createPaymentRequestDto.TotalAmountPrice = subTotalPrice;

        var paymentResponse = await _payOSService.CreatePaymentLinkAsync(createPaymentRequestDto, user);
        var orderId = await CreateOrder(createPaymentRequestDto, paymentResponse.Data.CheckoutUrl, request.GymCoursePTId);
        await CreateTransaction(paymentResponse, request.PaymentMethodId, orderId);
        
        return paymentResponse;
    }

    public async Task CreateTransaction(PaymentResponseDto paymentResponse, Guid paymentMethodId, Guid orderId)
    {
        var newTransaction = new Transaction
        {
            OrderCode = paymentResponse.Data.OrderCode,
            Description = "Payment for order " + paymentResponse.Data.OrderCode,
            PaymentMethodId = paymentMethodId,
            TransactionType = TransactionType.AssignPt,
            Status = TransactionStatus.Pending,
            OrderId = orderId,
            Amount = paymentResponse.Data.Amount
        };
        _unitOfWork.Repository<Transaction>().Insert(newTransaction);
        await _unitOfWork.CommitAsync();
    }

    public async Task<Guid> CreateOrder(CreatePaymentRequestDto request, string checkoutUrl, Guid gymCoursePTId)
    {
        var subTotalPrice = request.SubTotalPrice;
        var order = _mapper.Map<Order>(request);
        order.SubTotalPrice = subTotalPrice;
        order.Status = OrderStatus.PaymentProcessing;
        order.CheckoutUrl = checkoutUrl;
        order.GymCoursePTIdToAssign = gymCoursePTId;
        _unitOfWork.Repository<Order>().Insert(order);
        await _unitOfWork.CommitAsync();
        return order.Id;
    }

    public async Task GetAndValidateOrderItems(OrderItemDto item, Guid customerPurchasedIdToExtend)
    {
        var userPackage = await _unitOfWork.Repository<CustomerPurchased>().GetBySpecificationAsync(new GetCustomerPurchasedByIdSpec(customerPurchasedIdToExtend));

        if (userPackage == null)
        {
            throw new NotFoundException("Can't find customer purchased to extend");
        }
        var orderItemToExtend = userPackage.OrderItems.OrderByDescending(x => x.CreatedAt).First();
        if (orderItemToExtend.GymPtId != null)
        {
            throw new DataValidationFailedException("PT already exist in this customer purchased");
        }
        if (orderItemToExtend.GymCourseId != null)
        {
            var gymCoursePT = await _unitOfWork.Repository<GymCourse>().GetBySpecificationAsync(new GetGymCourseByIdSpecification(orderItemToExtend.GymCourseId.Value));

            if (gymCoursePT == null)
            {
                throw new NotFoundException("Gym course PT not found");
            }
            item.Price = gymCoursePT.PtPrice;
        }
        if (item.GymCoursePTId != null)
        {
            var gymCoursePT = await _unitOfWork.Repository<GymCoursePT>().GetByIdAsync(item.GymCoursePTId.Value);
            if (gymCoursePT == null)
            {
                throw new NotFoundException("Gym course PT not found");
            }
        }
    }
    
    public decimal CalculateSubTotalPrice(OrderItemDto OrderItem)
    {
        return OrderItem.Price * OrderItem.Quantity;
    }

}
