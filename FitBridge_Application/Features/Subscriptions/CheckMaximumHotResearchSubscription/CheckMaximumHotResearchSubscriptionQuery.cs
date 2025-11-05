using System;
using MediatR;
using FitBridge_Application.Dtos.Subscriptions;
namespace FitBridge_Application.Features.Subscriptions.CheckMaximumHotResearchSubscription;

public class CheckHotResearchSubscriptionQuery : IRequest<CheckHotResearchDto>
{
}
