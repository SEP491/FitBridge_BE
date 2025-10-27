using System;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Gym.GetAllGymOwnerCustomer;
using MediatR;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Interfaces.Repositories;
using AutoMapper;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Dtos;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Features.Gyms.GetAllGymCustomer;

public class GetAllGymOwnerCustomerQueryHandler(IUnitOfWork _unitOfWork, IUserUtil _userUtil, IHttpContextAccessor _httpContextAccessor, IMapper _mapper, IApplicationUserService _applicationUserService) : IRequestHandler<GetAllGymOwnerCustomerQuery, PagingResultDto<GetAllGymOwnerCustomer>>
{
    public async Task<PagingResultDto<GetAllGymOwnerCustomer>> Handle(GetAllGymOwnerCustomerQuery request, CancellationToken cancellationToken)
    {
        var userId = _userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var spec = new GetAllGymOwnerCustomerSpec(request.Params, userId.Value);
        var result = await _applicationUserService.GetAllUsersWithSpecAsync(spec);
        var getAllGymOwnerCustomerResult = new List<GetAllGymOwnerCustomer>();
        if(!result.Any()) {
            return new PagingResultDto<GetAllGymOwnerCustomer>(0, getAllGymOwnerCustomerResult);
        }
        foreach (var user in result)
        {
            var getAllGymOwnerCustomer = _mapper.Map<GetAllGymOwnerCustomer>(user);
            var latestCustomerPurchased = user.CustomerPurchased.Where(c => c.OrderItems.Any(o => o.GymCourseId != null && o.GymCourse!.GymOwnerId == userId.Value)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            var latestCustomerPurchasedOrderItem = latestCustomerPurchased.OrderItems.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            getAllGymOwnerCustomer.LatestCustomerPurchasedId = latestCustomerPurchased.Id;
            getAllGymOwnerCustomer.PackageName = latestCustomerPurchasedOrderItem.GymCourse?.Name;
            getAllGymOwnerCustomer.PtName = latestCustomerPurchasedOrderItem.GymPt?.FullName;
            getAllGymOwnerCustomer.ExpirationDate = latestCustomerPurchased.ExpirationDate;
            getAllGymOwnerCustomer.Status = latestCustomerPurchased.ExpirationDate > DateOnly.FromDateTime(DateTime.UtcNow) ? GymOwnerCustomerStatus.Active : GymOwnerCustomerStatus.Expired;
            getAllGymOwnerCustomer.JoinedAt = user.CustomerPurchased.OrderBy(x => x.CreatedAt).First().CreatedAt;
            getAllGymOwnerCustomer.PtGymAvailableSession = latestCustomerPurchased.AvailableSessions;
            getAllGymOwnerCustomer.AvatarUrl = user.AvatarUrl;
            getAllGymOwnerCustomerResult.Add(getAllGymOwnerCustomer);
        }
        var totalItems = await _applicationUserService.CountAsync(spec);
        return new PagingResultDto<GetAllGymOwnerCustomer>(totalItems, getAllGymOwnerCustomerResult);
    }
}
