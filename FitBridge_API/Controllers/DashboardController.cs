using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Features.Dashboards.GetAvailableBalanceDetail;
using FitBridge_Application.Features.Dashboards.GetPendingBalanceDetail;
using FitBridge_Application.Features.Dashboards.GetWalletBalance;
using FitBridge_Application.Specifications.Dashboards.GetAvailableBalanceDetail;
using FitBridge_Application.Specifications.Dashboards.GetPendingBalanceDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller that exposes endpoints for dashboard data.
    /// All endpoints return JSON wrapped in a <see cref="BaseResponse{T}"/> object.
    /// </summary>
    /// <remarks>
    /// Route: api/v{version:apiVersion}/Dashboard
    /// This controller provides endpoints to:
    /// - Retrieve wallet balance (pending and available).
    /// - Retrieve detailed available balance transactions.
    /// - Retrieve detailed pending balance orders.
    /// 
    /// Access: All endpoints are restricted to GymOwner and FreelancePT roles only.
    /// </remarks>
    [Authorize(Roles = $"{ProjectConstant.UserRoles.GymOwner},{ProjectConstant.UserRoles.FreelancePT}")]
    [Produces(MediaTypeNames.Application.Json)]
    public class DashboardController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves the wallet balance including both pending and available balance.
        /// </summary>
        /// <returns>
        /// A <see cref="BaseResponse{GetWalletBalanceDto}"/> containing the wallet balance information.
        /// Returns HTTP 200 with the data.
        /// </returns>
        /// <response code="200">Wallet balance retrieved successfully</response>
        /// <response code="401">Unauthorized - User must be authenticated</response>
        /// <response code="403">Forbidden - Only GymOwner and FreelancePT can access</response>
        [HttpGet("wallet-balance")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetWalletBalanceDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetWalletBalanceDto>> GetWalletBalance()
        {
            var response = await mediator.Send(new GetWalletBalanceQuery());

            return Ok(
                new BaseResponse<GetWalletBalanceDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Get wallet balance success",
                    response));
        }

        /// <summary>
        /// Retrieves detailed available balance transactions with pagination, search, and filtering support.
        /// </summary>
        /// <remarks>
        /// Paging is enabled by default. Use the 'DoApplyPaging' parameter to disable it.
        /// With paging enabled, page size = 10 by default, page number starts at 1.
        ///
        /// Filter options:
        /// - SearchTerm: partial/case-insensitive text match against gym course name or freelance PT package name.
        /// - TransactionType: filter by specific transaction type (only DistributeProfit or Withdraw allowed).
        /// - From: filter transactions created on or after this date (inclusive).
        /// - To: filter transactions created on or before this date (inclusive, includes entire day).
        ///
        /// Date range examples:
        /// - Single day: From=2024-01-15&amp;To=2024-01-15
        /// - Date range: From=2024-01-01&amp;To=2024-01-31
        /// - From date only: From=2024-01-01 (all transactions from this date onwards)
        /// - To date only: To=2024-01-31 (all transactions up to and including this date)
        /// </remarks>
        /// <param name="parameters">Query parameters for paging, filtering, and sorting. Includes: Page, Size, DoApplyPaging, SearchTerm, TransactionType (DistributeProfit/Withdraw only), From, To.</param>
        /// <returns>
        /// A <see cref="BaseResponse{Pagination{AvailableBalanceTransactionDto}}"/> containing paginated available balance transactions.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        /// <response code="200">Available balance details retrieved successfully</response>
        /// <response code="400">Bad Request - Invalid transaction type or parameters</response>
        /// <response code="401">Unauthorized - User must be authenticated</response>
        /// <response code="403">Forbidden - Only GymOwner and FreelancePT can access</response>
        [HttpGet("available-balance-detail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<AvailableBalanceTransactionDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Pagination<AvailableBalanceTransactionDto>>> GetAvailableBalanceDetail([FromQuery] GetAvailableBalanceDetailParams parameters)
        {
            var response = await mediator.Send(new GetAvailableBalanceDetailQuery(parameters));

            var pagedResult = ResultWithPagination(
                response.Items,
                response.Total,
                parameters.Page,
                parameters.Size);

            return Ok(
                new BaseResponse<Pagination<AvailableBalanceTransactionDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get available balance detail success",
                    pagedResult));
        }

        /// <summary>
        /// Retrieves detailed pending balance orders with pagination, search, and filtering support.
        /// </summary>
        /// <remarks>
        /// Paging is enabled by default. Use the 'DoApplyPaging' parameter to disable it.
        /// With paging enabled, page size = 10 by default, page number starts at 1.
        ///
        /// Filter options:
        /// - SearchTerm: partial/case-insensitive text match against gym course name or freelance PT package name.
        /// - From: filter order items created on or after this date (inclusive).
        /// - To: filter order items created on or before this date (inclusive, includes entire day).
        ///
        /// Date range examples:
        /// - Single day: From=2024-01-15&amp;To=2024-01-15
        /// - Date range: From=2024-01-01&amp;To=2024-01-31
        /// - From date only: From=2024-01-01 (all order items from this date onwards)
        /// - To date only: To=2024-01-31 (all order items up to and including this date)
        /// </remarks>
        /// <param name="parameters">Query parameters for paging, filtering, and sorting. Includes: Page, Size, DoApplyPaging, SearchTerm, From, To.</param>
        /// <returns>
        /// A <see cref="BaseResponse{Pagination{PendingBalanceOrderItemDto}}"/> containing paginated pending balance orders.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        /// <response code="200">Pending balance details retrieved successfully</response>
        /// <response code="401">Unauthorized - User must be authenticated</response>
        /// <response code="403">Forbidden - Only GymOwner and FreelancePT can access</response>
        [HttpGet("pending-balance-detail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<PendingBalanceOrderItemDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Pagination<PendingBalanceOrderItemDto>>> GetPendingBalanceDetail([FromQuery] GetPendingBalanceDetailParams parameters)
        {
            var response = await mediator.Send(new GetPendingBalanceDetailQuery(parameters));

            var pagedResult = ResultWithPagination(
                response.Items,
                response.Total,
                parameters.Page,
                parameters.Size);

            return Ok(
                new BaseResponse<Pagination<PendingBalanceOrderItemDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get pending balance detail success",
                    pagedResult));
        }
    }
}