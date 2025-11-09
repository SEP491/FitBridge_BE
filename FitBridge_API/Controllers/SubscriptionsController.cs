using FitBridge_Application.Features.Subscriptions;
using FitBridge_Application.Features.Subscriptions.GetSubscriptionPlans;
using FitBridge_Application.Features.Subscriptions.GetUserSubscriptionHistory;
using FitBridge_Application.Specifications.Subscriptions.GetUserSubscriptionHistory;
using FitBridge_API.Helpers.RequestHelpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitBridge_Application.Dtos.Subscriptions;
using FitBridge_Application.Features.Subscriptions.CancelSubscription;
using FitBridge_Application.Features.Subscriptions.CheckMaximumHotResearchSubscription;

namespace FitBridge_API.Controllers;

public class SubscriptionsController(IMediator mediator) : _BaseApiController
{
    [HttpGet("plans")]
    public async Task<IActionResult> GetSubscriptionPlans()
    {
        var query = new GetSubscriptionPlansQuery();
        var result = await mediator.Send(query);
        return Ok(new BaseResponse<List<SubscriptionPlanResponseDto>>(StatusCodes.Status200OK.ToString(), "Subscription plans retrieved successfully", result));
    }

    [HttpGet("user-subscription/history")]
    public async Task<IActionResult> GetUserSubscriptionHistory([FromQuery] GetUserSubscriptionHistoryParams Params)
    {
        var result = await mediator.Send(new GetUserSubscriptionHistoryQuery { Params = Params });
        var pagination = ResultWithPagination(result.Items, result.Total, Params.Page, Params.Size);
        return Ok(new BaseResponse<Pagination<UserSubscriptionHistoryResponseDto>>(StatusCodes.Status200OK.ToString(), "User subscription history retrieved successfully", pagination));
    }

    [HttpPut("cancel-subscription/{userSubscriptionId}")]
    public async Task<IActionResult> CancelSubscription([FromRoute] Guid userSubscriptionId)
    {
        var result = await mediator.Send(new CancelSubscriptionCommand { userSubscriptionId = userSubscriptionId });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Subscription cancelled successfully", result));
    }

    [HttpGet("check-hot-research-subscription")]
    public async Task<IActionResult> CheckHotResearchSubscription()
    {
        var result = await mediator.Send(new CheckHotResearchSubscriptionQuery());
        return Ok(new BaseResponse<CheckHotResearchDto>(StatusCodes.Status200OK.ToString(), "Maximum hot research subscription checked successfully", result));
    }
}
