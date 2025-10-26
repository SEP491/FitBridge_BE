using System;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FitBridge_Application.Features.Jobs.DistributeProfit;
using FitBridge_API.Helpers.RequestHelpers;

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
}
