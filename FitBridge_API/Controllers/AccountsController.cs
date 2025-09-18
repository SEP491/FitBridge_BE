using System;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Features.Accounts.GetUserProfile;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class AccountsController(IMediator _mediator, IUserUtil _userUtil) : _BaseApiController
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var accountId = _userUtil.GetAccountId(HttpContext);
        if (accountId == null)
        {
            return Unauthorized(new BaseResponse<string>(StatusCodes.Status401Unauthorized.ToString(), "Unauthorized", null));
        }

        var response = await _mediator.Send(new GetProfileCommand { AccountId = accountId });
        return Ok(new BaseResponse<GetUserProfileResponse>(StatusCodes.Status200OK.ToString(), "Profile retrieved successfully", response));
    }
}
