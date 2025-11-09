using System;
using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Subscriptions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Subscriptions.GetUserSubscriptionHistory;
using FitBridge_Domain.Entities.ServicePackages;
using MediatR;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
    
namespace FitBridge_Application.Features.Subscriptions.GetUserSubscriptionHistory;

public class GetUserSubscriptionHistoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserUtil userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetUserSubscriptionHistoryQuery, PagingResultDto<UserSubscriptionHistoryResponseDto>>
{
    public async Task<PagingResultDto<UserSubscriptionHistoryResponseDto>> Handle(GetUserSubscriptionHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }
        var userSubscriptionHistory = await unitOfWork.Repository<UserSubscription>().GetAllWithSpecificationProjectedAsync<UserSubscriptionHistoryResponseDto>(new GetUserSubscriptionHistorySpec(request.Params, userId.Value), mapper.ConfigurationProvider);

        var totalItems = await unitOfWork.Repository<UserSubscription>().CountAsync(new GetUserSubscriptionHistorySpec(request.Params, userId.Value));
        
        return new PagingResultDto<UserSubscriptionHistoryResponseDto>(totalItems, userSubscriptionHistory);
    }

}
