using System;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymCoursePts.GetGymCoursePtById;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByGymId;
using FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;
using FitBridge_Application.Specifications.Vouchers;
using FitBridge_Application.Specifications.GymCourses.GetGymCourseById;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Accounts;

namespace FitBridge_Application.Features.Orders.CreateOrders;

public class CreateOrderCommandHandler(IMapper _mapper, IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IApplicationUserService _applicationUserService) : IRequestHandler<CreateOrderCommand, string>
{
    public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        await GetAndValidateOrderItems(request.OrderItems);
        var totalPrice = CalculateTotalPrice(request.OrderItems);
        var subTotalPrice = await CalculateSubTotalPrice(request, totalPrice);
        request.AccountId = userId.Value;
        var order = _mapper.Map<Order>(request);
        order.TotalAmount = totalPrice;
        order.SubTotalPrice = subTotalPrice;
        order.Status = OrderStatus.PaymentProcessing;

        _unitOfWork.Repository<Order>().Insert(order);
        await _unitOfWork.CommitAsync();
        return order.Id.ToString();
    }

    public async Task GetAndValidateOrderItems(List<OrderItemDto> OrderItems)
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
                }

                var userPackage = await _unitOfWork.Repository<CustomerPurchased>().GetBySpecificationAsync(new GetCustomerPurchasedByGymIdSpec(gymCoursePT.GymOwnerId));
                if (userPackage != null)
                {
                    throw new PackageExistException("Package of this gym course still not expired");
                }
                item.Price = gymCoursePT.Price;

            }

            if (item.FreelancePTPackageId != null)
            {
                var freelancePTPackage = await _unitOfWork.Repository<FreelancePTPackage>().GetBySpecificationAsync(new GetFreelancePtPackageByIdSpec(item.FreelancePTPackageId.Value));
                if (freelancePTPackage == null)
                {
                    throw new NotFoundException("Freelance PTPackage not found");
                }
                item.Price = freelancePTPackage.Price;
                var userPackage = await _unitOfWork.Repository<CustomerPurchased>().GetBySpecificationAsync(new GetCustomerPurchasedByFreelancePtIdSpec(freelancePTPackage.PtId));
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

    public async Task<decimal> CalculateSubTotalPrice(CreateOrderCommand request, decimal totalPrice)
    {
        if (request.VoucherId != null)
        {
            var voucher = await _unitOfWork.Repository<Voucher>().GetBySpecificationAsync(new GetVoucherByIdSpec(request.VoucherId!.Value));
            if (voucher == null)
            {
                throw new NotFoundException("Voucher not found");
            }
            var voucherDiscountAmount = (decimal)voucher.DiscountPercent / 100 * totalPrice > voucher.MaxDiscount ? voucher.MaxDiscount : (decimal)voucher.DiscountPercent / 100 * totalPrice;
            return totalPrice - voucherDiscountAmount + request.ShippingFee;
        }

        return totalPrice + request.ShippingFee;
    }
}
