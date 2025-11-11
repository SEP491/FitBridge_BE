using System;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.ServicePackages;
using Quartz;
using FitBridge_Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Application.Interfaces.Services.Notifications;

namespace FitBridge_Infrastructure.Jobs.Subscriptions;

public class SendRemindExpiredSubscriptionNotiJob(IUnitOfWork _unitOfWork, ILogger<SendRemindExpiredSubscriptionNotiJob> _logger, INotificationService _notificationService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var userSubscriptionId = Guid.Parse(context.JobDetail.JobDataMap.GetString("userSubscriptionId") ?? throw new NotFoundException($"{nameof(SendRemindExpiredSubscriptionNotiJob)}_userSubscriptionId"));
        var userSubscription = await _unitOfWork.Repository<UserSubscription>().GetByIdAsync(userSubscriptionId);
        if (userSubscription == null)
        {
            _logger.LogError($"User subscription not found for user subscription id {userSubscriptionId}");
            return;
        }
        var daysRemaining = (userSubscription.EndDate - DateTime.UtcNow.Date).Days;
        var messageModel = new NearExpireSubscriptionModel(userSubscription.SubscriptionPlansInformation.PlanName, daysRemaining);
        var message = new NotificationMessage(
            EnumContentType.NearExpiredSubscriptionReminder,
            [userSubscription.UserId],
            messageModel);
        await _notificationService.NotifyUsers(message);
    }
}
