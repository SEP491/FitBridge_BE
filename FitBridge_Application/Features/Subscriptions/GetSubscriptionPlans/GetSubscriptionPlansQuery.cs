
using FitBridge_Application.Dtos.Subscriptions;
using MediatR;

namespace FitBridge_Application.Features.Subscriptions.GetSubscriptionPlans;

public class GetSubscriptionPlansQuery : IRequest<List<SubscriptionPlanResponseDto>>
{
}
