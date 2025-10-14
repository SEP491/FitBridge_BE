using System;
using System.Runtime.CompilerServices;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Features.Accounts.GetAllFreelancePts;
using FitBridge_Application.Features.Accounts.GetFreelancePtById;
using FitBridge_Application.Features.Accounts.GetFreelancePtCustomers;
using FitBridge_Application.Features.Accounts.GetUserProfile;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedFreelancePt;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Accounts.GetAllFreelancePts;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetFreelancePtCustomerPurchased;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

/// <summary>
/// Controller for managing user accounts, including profile retrieval and customer management.
/// </summary>
[Authorize]
public class AccountsController(IMediator _mediator, IUserUtil _userUtil) : _BaseApiController
{
    /// <summary>
    /// Retrieves the authenticated user's profile information.
    /// </summary>
    /// <returns>The user's profile details including personal information and settings.</returns>
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetUserProfileResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(BaseResponse<string>))]
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

    /// Retrieves a paginated list of customers who have purchased packages from the authenticated freelance PT.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and pagination, including:
    /// <list type="bullet">
    /// <item>
    /// <term>Page</term>
    /// <description>The page number to retrieve (default: 1).</description>
    /// </item>
    /// <item>
    /// <term>Size</term>
    /// <description>The number of items per page (default: 10).</description>
    /// </item>
    /// <item>
    /// <term>SearchTerm</term>
    /// <description>Optional search term to filter customers by name or email.</description>
    /// </item>
    /// <item>
    /// <term>DoApplyPaging</term>
    /// <description>Whether to apply pagination (default: true).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>A paginated list of customers who purchased the PT's packages.</returns>
    [HttpGet("freelance-pt/customers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetCustomersDto>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFreelancePtCustomers([FromQuery] GetFreelancePtCustomerPurchasedParams parameters)
    {
        var response = await _mediator.Send(new GetFreelancePtCustomerQuery(parameters));

        var pagedResult = new Pagination<GetCustomersDto>(
            response.Items,
            response.Total,
            parameters.Page,
            parameters.Size);

        return Ok(
            new BaseResponse<Pagination<GetCustomersDto>>(
                StatusCodes.Status200OK.ToString(),
                "Get freelance PT customers success",
                pagedResult));
    }
}