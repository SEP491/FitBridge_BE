using System;
using System.Runtime.CompilerServices;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Features.Accounts.GetAllFreelancePts;
using FitBridge_Application.Features.Accounts.GetFreelancePtById;
using FitBridge_Application.Features.Accounts.GetUserProfile;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Accounts.GetAllFreelancePts;
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

    /// <summary>
    /// Get freelance pts for customer to browse and purchase
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("freelance-pts")]
    public async Task<IActionResult> GetFreelancePTs([FromQuery] GetAllFreelancePTsParam parameters)
    {
        var response = await _mediator.Send(new GetAllFreelancePTsQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetAllFreelancePTsResponseDto>>(StatusCodes.Status200OK.ToString(), "Freelance PTs retrieved successfully", pagination));
    }
    /// <summary>
    /// Get freelance pt detail for customer to view details information about freelance pt and their packages
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("freelance-pt/{id}")]
    public async Task<IActionResult> GetFreelancePTById([FromRoute] Guid id)
    {
        var response = await _mediator.Send(new GetFreelancePTByIdQuery { Id = id });
        return Ok(new BaseResponse<GetFreelancePtByIdResponseDto>(StatusCodes.Status200OK.ToString(), "Freelance PT retrieved successfully", response));
    }
}
