using System;
using MediatR;
using FitBridge_Application.Services;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Subscriptions;

namespace FitBridge_Application.Features.Subscriptions.CheckMaximumHotResearchSubscription;

public class CheckHotResearchSubscriptionQueryHandler(SubscriptionService subscriptionService, SystemConfigurationService systemConfigurationService) : IRequestHandler<CheckHotResearchSubscriptionQuery, CheckHotResearchDto>
{
    public async Task<CheckHotResearchDto> Handle(CheckHotResearchSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var maxHotResearchSubscription = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.HotResearchSubscriptionLimit);
        var numOfHotResearchSubscription = await subscriptionService.GetNumOfCurrentHotResearchSubscription();
        if (numOfHotResearchSubscription >= maxHotResearchSubscription)
        {
            return new CheckHotResearchDto { IsHotResearchSubscriptionAvailable = false, NumOfCurrentHotResearchSubscription = numOfHotResearchSubscription, MaxHotResearchSubscription = maxHotResearchSubscription };
        }
        return new CheckHotResearchDto { IsHotResearchSubscriptionAvailable = true, NumOfCurrentHotResearchSubscription = numOfHotResearchSubscription, MaxHotResearchSubscription = maxHotResearchSubscription };
    }

}
