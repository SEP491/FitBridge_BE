using System;
using AutoMapper;
using FitBridge_Application.Dtos.Subscriptions;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Subscriptions.GetAllSubscriptionPlans;
using FitBridge_Domain.Entities.ServicePackages;
using MediatR;
using Microsoft.AspNetCore.Http;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.SubscriptionPlans;

namespace FitBridge_Application.Features.Subscriptions.GetSubscriptionPlans;

public class GetSubscriptionPlansQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserUtil userUtil, IHttpContextAccessor _httpContextAccessor) : IRequestHandler<GetSubscriptionPlansQuery, List<SubscriptionPlanResponseDto>>
{
    public async Task<List<SubscriptionPlanResponseDto>> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
    {
        var userId = userUtil.GetAccountId(_httpContextAccessor.HttpContext);
        if (userId == null)
        {
            throw new NotFoundException("User not found");
        }

        var subscriptionPlans = await unitOfWork.Repository<SubscriptionPlansInformation>().GetAllWithSpecificationAsync(new GetAllSubscriptionPlansSpecification(request.IsGetHotResearchSubscription));
        var subscriptionPlanDtos = new List<SubscriptionPlanResponseDto>();
        foreach (var subscriptionPlan in subscriptionPlans)
        {
            var subscriptionPlanDto = mapper.Map<SubscriptionPlanResponseDto>(subscriptionPlan);
            var currentActiveUserSubscription = subscriptionPlan.UserSubscriptions.FirstOrDefault(x => x.UserId == userId.Value && x.Status == SubScriptionStatus.Active);
            subscriptionPlanDto.IsActiveForCurrentUser = false;
            if (currentActiveUserSubscription != null)
            {
                subscriptionPlanDto.IsActiveForCurrentUser = true;
                subscriptionPlanDto.CurrentUserSubscription = mapper.Map<CurrentUserSubscriptionResponseDto>(currentActiveUserSubscription);
            }
            subscriptionPlanDtos.Add(subscriptionPlanDto);
        }
        return subscriptionPlanDtos;
    }

}
