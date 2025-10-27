using System;
using MediatR;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Specifications.Gym.GetGymOwnerCustomerById;
using FitBridge_Domain.Enums.Gyms;
using FitBridge_Application.Dtos.OrderItems;

namespace FitBridge_Application.Features.Gyms.GetGymOwnerCustomerById;

public class GetGymOwnerCustomerByIdQueryHandler(IApplicationUserService _applicationUserService, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IMapper _mapper) : IRequestHandler<GetGymOwnerCustomerByIdQuery, GetGymOwnerCustomerDetail>
{
    public async Task<GetGymOwnerCustomerDetail> Handle(GetGymOwnerCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var spec = new GetGymOwnerCustomerByIdSpec(request.Id, userId.Value);
        var result = await _applicationUserService.GetUserWithSpecAsync(spec);
        var customerDetail = _mapper.Map<GetGymOwnerCustomerDetail>(result);
        
        var latestCustomerPurchased = result.CustomerPurchased.Where(c => c.OrderItems.Any(o => o.GymCourseId != null && o.GymCourse!.GymOwnerId == userId.Value)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        var latestCustomerPurchasedOrderItem = latestCustomerPurchased.OrderItems.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        customerDetail.LatestCustomerPurchasedId = latestCustomerPurchased.Id;
        customerDetail.PackageName = latestCustomerPurchasedOrderItem.GymCourse?.Name;
        customerDetail.PtName = latestCustomerPurchasedOrderItem.GymPt?.FullName;
        customerDetail.PtAvatarUrl = latestCustomerPurchasedOrderItem.GymPt?.AvatarUrl;
        customerDetail.AvatarUrl = result.AvatarUrl;
        customerDetail.ExpirationDate = latestCustomerPurchased.ExpirationDate;
        customerDetail.Status = latestCustomerPurchased.ExpirationDate > DateOnly.FromDateTime(DateTime.UtcNow) ? GymOwnerCustomerStatus.Active : GymOwnerCustomerStatus.Expired;
        customerDetail.JoinedAt = result.CustomerPurchased.OrderBy(x => x.CreatedAt).First().CreatedAt;
        customerDetail.PtGymAvailableSession = latestCustomerPurchased.AvailableSessions;
        customerDetail.OrderItemsList = new List<OrderItemForGymOwnerCust>();

        var customerPurchasedOfThisGymOwner = result.CustomerPurchased.Where(c => c.OrderItems.Any(o => o.GymCourseId != null && o.GymCourse!.GymOwnerId == userId.Value)).ToList();
        foreach (var customerPurchased in customerPurchasedOfThisGymOwner)
        {
            foreach (var orderItem in customerPurchased.OrderItems)
            {
                var orderItemDto = new OrderItemForGymOwnerCust
                {
                    OrderItemId = orderItem.Id,
                    GymCourseId = orderItem.GymCourseId,
                    GymPtId = orderItem.GymPtId,
                    CourseName = orderItem.GymCourse?.Name,
                    PtName = orderItem.GymPt?.FullName,
                    PtImageUrl = orderItem.GymPt?.AvatarUrl,
                    CustomerPurchasedId = customerPurchased.Id,
                };
                var coupon = orderItem.Order.Coupon?.DiscountPercent;
                if (coupon != null)
                {
                    var totalOrderItemPrice = orderItem.Price * orderItem.Quantity;
                    var discountAmount = totalOrderItemPrice * (decimal)coupon / 100;
                    orderItemDto.AmountSpend = totalOrderItemPrice - discountAmount;
                }
                else
                {
                    orderItemDto.AmountSpend = orderItem.Price * orderItem.Quantity;
                }
                customerDetail.OrderItemsList.Add(orderItemDto);
            }
        }
        return customerDetail;
    }

}