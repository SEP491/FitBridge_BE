using System;
using System.Security.Cryptography.X509Certificates;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Subscriptions.GetHotResearchSubscription;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Enums.SubscriptionPlans;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Services;

public class SubscriptionService(IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService, SystemConfigurationService _systemConfigurationService)
{
    public async Task<bool> ExpireUserSubscription(Guid userSubscriptionId)
    {
        var userSubscription = await _unitOfWork.Repository<UserSubscription>().GetByIdAsync(userSubscriptionId, false, includes: new List<string> { "User", "SubscriptionPlansInformation", "SubscriptionPlansInformation.FeatureKey" });
        if (userSubscription == null)
        {
            throw new NotFoundException($"User subscription not found for user subscription {userSubscriptionId}");
        }
        userSubscription.Status = SubScriptionStatus.Expired;
        userSubscription.UpdatedAt = DateTime.UtcNow;
        var featureKey = userSubscription.SubscriptionPlansInformation.FeatureKey;
        if (featureKey.FeatureName == ProjectConstant.FeatureKeyNames.HotResearch)
        {
            await RevokeHotResearchSubscriptionPlanBenefit(userSubscription.User);
        }
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task RevokeHotResearchSubscriptionPlanBenefit(ApplicationUser user)
    {
        user.hotResearch = false;
        user.UpdatedAt = DateTime.UtcNow;
    }

    public async Task<int> GetNumOfCurrentHotResearchSubscription()
    {
        var numOfHotResearchSubscription = await _unitOfWork.Repository<UserSubscription>().CountAsync(new GetHotResearchSubscriptionSpecification());
        return numOfHotResearchSubscription;
    }
}
