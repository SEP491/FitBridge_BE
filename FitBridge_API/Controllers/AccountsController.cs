using System;
using System.Runtime.CompilerServices;
using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Dtos.Accounts.HotResearch;
using FitBridge_Application.Dtos.GymPTs;
using FitBridge_Application.Features.Accounts.BanUnbanAccounts;
using FitBridge_Application.Features.Accounts.DeleteAccounts;
using FitBridge_Application.Features.Accounts.GetAllFreelancePts;
using FitBridge_Application.Features.Accounts.GetAllGymPts;
using FitBridge_Application.Features.Accounts.GetFreelancePtById;
using FitBridge_Application.Features.Accounts.GetHotResearchAccount;
using FitBridge_Application.Features.Accounts.GetFreelancePtCustomers;
using FitBridge_Application.Features.Accounts.GetUserProfile;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedFreelancePt;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Accounts.GetAllFreelancePts;
using FitBridge_Application.Specifications.Accounts.GetAllGymPts;
using FitBridge_Application.Specifications.Accounts.GetHotResearchAccount;
using FitBridge_Application.Specifications.Accounts.GetFreelancePtCustomers;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetFreelancePtCustomerPurchased;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitBridge_Application.Specifications.Accounts.GetAccountForSearching;
using FitBridge_Application.Features.Accounts.SearchAccounts;
using FitBridge_Application.Dtos.Accounts.Search;
using FitBridge_Application.Features.Accounts.UpdateProfiles;
using FitBridge_Application.Dtos.Accounts.Profiles;
using FitBridge_Application.Features.Accounts.UpdateAvatar;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Accounts.GetAllGymPtsForAdmin;
using FitBridge_Application.Features.Accounts.GetAllGymPtsForAdmin;
using FitBridge_Application.Features.Accounts.GetGymPTByIdForAdmin;
using FitBridge_Application.Specifications.Accounts.GetAllGymOwnerForAdmin;
using FitBridge_Application.Features.Accounts.GetAllGymOwnerForAdmin;
using FitBridge_Application.Features.Accounts.GetGymOwnerByIdForAdmin;
using FitBridge_Application.Dtos.Accounts.Customers;
using FitBridge_Application.Specifications.Accounts.GetAllCustomersForAdmin;
using FitBridge_Application.Features.Accounts.GetAllCustomersForAdmin;
using FitBridge_Application.Features.GymSlots.GetGymPtRegisterSlotForGymOwner;
using FitBridge_Application.Specifications.GymSlotPts.GetGymPtRegisterSlotForGymOwner;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.GymSlotPts.GetGymSlotPtBooking;
using FitBridge_Application.Features.GymSlots.GetGymSlotPtBooking;

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
    [AllowAnonymous]
    public async Task<IActionResult> GetFreelancePTs([FromQuery] GetAllFreelancePTsParam parameters)
    {
        var response = await _mediator.Send(new GetAllFreelancePTsQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetAllFreelancePTsResponseDto>>(StatusCodes.Status200OK.ToString(), "Freelance PTs retrieved successfully", pagination));
    }

    /// <summary>
    /// Retrieves a paginated list of gym personal trainers.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and pagination, including:
    /// <list type="bullet">
    /// <item>
    /// <term>Page</term>
    /// <description>The page number to retrieve (default: 1).</description>
    /// </item>
    /// <item>
    /// <term>Size</term>
    /// <description>The number of items per page (default: 10, max: 20).</description>
    /// </item>
    /// <item>
    /// <term>SearchTerm</term>
    /// <description>Optional search term to filter gym PTs by name.</description>
    /// </item>
    /// <item>
    /// <term>SortBy</term>
    /// <description>Field to sort by (e.g., FullName, Rating, Experience).</description>
    /// </item>
    /// <item>
    /// <term>SortOrder</term>
    /// <description>Sort direction (asc or desc).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>A paginated list of gym personal trainers with their details.</returns>
    /// <remarks>
    /// Returns gym PT information including:
    /// - Basic info (name, avatar, bio, date of birth)
    /// - Professional details (experience, goal trainings)
    /// - Performance metrics (rating, total courses assigned)
    /// - Gym association (gym owner ID, gym name)
    /// 
    /// Sample request:
    ///
    ///     GET /api/v1/accounts/gym-pts?page=1&amp;size=10&amp;sortBy=Rating&amp;sortOrder=desc
    ///
    /// </remarks>
    /// <response code="200">Gym PTs retrieved successfully</response>
    /// <response code="400">Invalid query parameters</response>
    [HttpGet("gym-pts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetAllGymPtsResponseDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllGymPTs([FromQuery] GetAllGymPtsParams parameters)
    {
        var response = await _mediator.Send(new GetAllGymPtsQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetAllGymPtsResponseDto>>(StatusCodes.Status200OK.ToString(), "Gym PTs retrieved successfully", pagination));
    }

    /// <summary>
    /// Get freelance pt detail for customer to view details information about freelance pt and their packages
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("freelance-pt/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFreelancePTById([FromRoute] Guid id)
    {
        var response = await _mediator.Send(new GetFreelancePTByIdQuery { Id = id });
        return Ok(new BaseResponse<GetFreelancePtByIdResponseDto>(StatusCodes.Status200OK.ToString(), "Freelance PT retrieved successfully", response));
    }

    /// <summary>
    /// API endpoint to retrieve a paginated list of hot research accounts, include personal trainers and gyms
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns>HotResearchAccountDto with field UserRole can be "" because data is added manually into database instead of using identity framework</returns>
    [HttpGet("hot-research")]
    public async Task<IActionResult> GetHotResearch([FromQuery] GetHotResearchAccountParams parameters)
    {
        var response = await _mediator.Send(new GetHotResearchAccountQuery
        {
            Params = parameters
        });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<HotResearchAccountDto>>(StatusCodes.Status200OK.ToString(), "Hot research retrieved successfully", pagination));
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
    public async Task<IActionResult> GetFreelancePtCustomers([FromQuery] GetFreelancePtCustomerParams parameters)
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

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchAccounts([FromQuery] GetAccountForSearchingParams parameters)
    {
        var response = await _mediator.Send(new SearchAccountQuery { Params = parameters });
        var result = new
        {
            FreelancePts = ResultWithPagination(response.FreelancePts.Items, response.FreelancePts.Total, parameters.Page, parameters.Size),
            Gyms = ResultWithPagination(response.Gyms.Items, response.Gyms.Total, parameters.Page, parameters.Size)
        };
        return Ok(new BaseResponse<object>(StatusCodes.Status200OK.ToString(), "Accounts searched successfully", result));
    }

    /// <summary>
    /// Update the authenticated user's profile information.
    /// </summary>
    /// <param name="accountId">The ID of the account to update.</param>
    /// <param name="taxCode">The tax code of the account to update, it is unique.</param>
    /// <param name="command">The command containing the updated profile information.</param>
    /// <returns>The updated profile information.</returns>
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(new BaseResponse<UpdateProfileResponseDto>(StatusCodes.Status200OK.ToString(), "Profile updated successfully", response));
    }

    [HttpPut("update-avatar")]
    public async Task<IActionResult> UpdateAvatar([FromForm] UpdateAvatarCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Avatar updated successfully", response));
    }

    /// <summary>
    /// Ban or unban user accounts. 
    /// Admin can ban/unban any accounts. Gym owners can only ban/unban their gym PTs.
    /// </summary>
    /// <param name="command">The command containing:
    /// <list type="bullet">
    /// <item>
    /// <term>UserIdBanUnbanList</term>
    /// <description>List of user IDs to ban/unban.</description>
    /// </item>
    /// <item>
    /// <term>IsBan</term>
    /// <description>True to ban (set IsActive = false), false to unban (set IsActive = true).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>Success response when accounts are banned/unbanned.</returns>
    [HttpPut("ban-unban")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin + "," + ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<object>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BanUnbanAccounts([FromBody] BanUnbanAccountCommand command)
    {
        await _mediator.Send(command);
        return Ok(new BaseResponse<object>(
        StatusCodes.Status200OK.ToString(),
        command.IsBan ? "Accounts banned successfully" : "Accounts unbanned successfully",
        null));
    }

    /// <summary>
    /// Soft delete user accounts by setting IsEnabled = false.
    /// Admin can delete any accounts. Gym owners can only delete their gym PTs.
    /// </summary>
    /// <param name="command">The command containing:
    /// <list type="bullet">
    /// <item>
    /// <term>UserIdDeleteList</term>
    /// <description>List of user IDs to delete.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>Success response when accounts are deleted.</returns>
    [HttpDelete("delete")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin + "," + ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<object>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccounts([FromBody] DeleteAccountCommand command)
    {
        await _mediator.Send(command);
        return Ok(new BaseResponse<object>(
       StatusCodes.Status200OK.ToString(),
          "Accounts deleted successfully",
         null));
    }

    /// <summary>
    /// Get all gym PTs for admin to view list of gym PTs and their gym owner
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("admin/gym-pts")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetAllGymPtsForAdminResponseDto>>))]
    public async Task<IActionResult> GetAllGymPTsForAdmin([FromQuery] GetAllGymPtsForAdminParams parameters)
    {
        var response = await _mediator.Send(new GetAllGymPtsForAdminQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetAllGymPtsForAdminResponseDto>>(StatusCodes.Status200OK.ToString(), "Gym PTs retrieved successfully", pagination));
    }

    /// <summary>
    /// Get gym PT detail for admin to view details information about gym PT and their gym owner
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("admin/gym-pt/{id}")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetGymPtsDetailForAdminResponseDto>))]
    public async Task<IActionResult> GetGymPTByIdForAdmin([FromRoute] Guid id)
    {
        var response = await _mediator.Send(new GetGymPTByIdForAdminQuery { Id = id });
        return Ok(new BaseResponse<GetGymPtsDetailForAdminResponseDto>(StatusCodes.Status200OK.ToString(), "Gym PT retrieved successfully", response));
    }

    /// <summary>
    /// Get all gym owners for admin to view list of gym owners and their gym PTs
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("admin/gym-owners")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
    public async Task<IActionResult> GetAllGymOwnersForAdmin([FromQuery] GetAllGymOwnerForAdminParams parameters)
    {
        var response = await _mediator.Send(new GetAllGymOwnerForAdminQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetAllGymOwnerForAdminDto>>(StatusCodes.Status200OK.ToString(), "Gym owners retrieved successfully", pagination));
    }

    [HttpGet("admin/gym-owner/{id}")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetGymOwnerDetailForAdminDto>))]
    public async Task<IActionResult> GetGymOwnerByIdForAdmin([FromRoute] Guid id)
    {
        var response = await _mediator.Send(new GetGymOwnerByIdForAdminQuery { Id = id });
        return Ok(new BaseResponse<GetGymOwnerDetailForAdminDto>(StatusCodes.Status200OK.ToString(), "Gym owner retrieved successfully", response));
    }

    /// <summary>
    /// Get all customers for admin to view list of customers and their purchases
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns>A paginated list of customers with their details.</returns>
    [HttpGet("admin/customers")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
    public async Task<IActionResult> GetAllCustomersForAdmin([FromQuery] GetAllCustomersForAdminParams parameters)
    {
        var response = await _mediator.Send(new GetAllCustomersForAdminQuery { Params = parameters });
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetAllCustomersForAdminDto>>(StatusCodes.Status200OK.ToString(), "Customers retrieved successfully", pagination));
    }

    /// <summary>
    /// Get all gym PT register slots for gym owner to view list of gym PT registered slots in a period of time
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("gym-owner/gym-pt-register-slots")]
    public async Task<IActionResult> GetGymPtRegisterSlotsForGymOwner([FromQuery] GetGymPtRegisterSlotForGymOwnerParams parameters)
    {
        var response = await _mediator.Send(new GetGymPtRegisterSlotForGymOwnerQuery(parameters));
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GymPtRegisterSlot>>(StatusCodes.Status200OK.ToString(), "Gym PT register slots retrieved successfully", pagination));
    }

    /// <summary>
    /// Get all gym PT bookings for gym owner to view list of gym PT register slots that are booked by customers
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("gym-owner/gym-pt-bookings")]
    public async Task<IActionResult> GetGymPtBookingsForGymOwner([FromQuery] GetGymSlotPtBookingParams parameters)
    {
        var response = await _mediator.Send(new GetGymSlotPtBookingQuery(parameters));
        var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GymSlotPtBookingDto>>(StatusCodes.Status200OK.ToString(), "Gym PT bookings retrieved successfully", pagination));
    }
}