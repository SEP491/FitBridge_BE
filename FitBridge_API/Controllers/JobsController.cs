using System;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FitBridge_Application.Features.Jobs.DistributeProfit;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Features.Jobs.ExpiredSubscription;
using FitBridge_Application.Features.Jobs.RemindSubscription;
using FitBridge_Application.Features.Jobs.TriggerInstantJob;

namespace FitBridge_API.Controllers;

public class JobsController(IMediator mediator) : _BaseApiController
{
    /// <summary>
    /// Schedule a job to distribute profit
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("schedule-distribute-profit")]
    public async Task<IActionResult> ScheduleDistributeProfit([FromBody] DistributeProfitCommand command)
    {
        var response = await mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Profit distributed successfully", response));
    }

    [HttpPost("expire-user-subscription")]
    public async Task<IActionResult> TriggerExpireUserSubscriptionJob([FromBody] TriggerExpireUserSubscriptionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "User subscription expired successfully", result));
    }

    [HttpPost("send-remind-expired-subscription-noti")]
    public async Task<IActionResult> TriggerSendRemindExpiredSubscriptionNotiJob([FromBody] TriggerSendRemindExpiredSubscriptionNotiCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "User subscription expired successfully", result));
    }

    [HttpPost("trigger-instant-job")]
    public async Task<IActionResult> TriggerInstantJob([FromBody] InstantTriggerJobCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Instant job triggered successfully", result));
    }
}
