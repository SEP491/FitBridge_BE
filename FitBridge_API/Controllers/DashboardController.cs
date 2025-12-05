using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Features.Dashboards.GetAvailableBalanceDetail;
using FitBridge_Application.Features.Dashboards.GetPendingBalanceDetail;
using FitBridge_Application.Features.Dashboards.GetRevenueDetail;
using FitBridge_Application.Features.Dashboards.GetWalletBalance;
using FitBridge_Application.Specifications.Dashboards.GetPendingBalanceDetail;
using FitBridge_Application.Specifications.Dashboards.GetOrderItemForRevenueDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using FitBridge_Application.Specifications.Dashboards.GetTransactionForAvailableBalanceDetail;

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
    /// - Retrieve detailed revenue information for all order items.
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
        /// 
        /// The response includes TotalProfitSum which is the sum of all TotalProfit values in the result set.
        /// </remarks>
        /// <param name="parameters">Query parameters for paging, filtering, and sorting. Includes: Page, Size, DoApplyPaging, SearchTerm, TransactionType (DistributeProfit/Withdraw only), From, To.</param>
        /// <returns>
        /// A <see cref="BaseResponse{DashboardPagingResultDto{AvailableBalanceTransactionDto}}"/> containing paginated available balance transactions with profit sum.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        /// <response code="200">Available balance details retrieved successfully</response>
        /// <response code="400">Bad Request - Invalid transaction type or parameters</response>
        /// <response code="401">Unauthorized - User must be authenticated</response>
        /// <response code="403">Forbidden - Only GymOwner and FreelancePT can access</response>
        [HttpGet("available-balance-detail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<DashboardPagingResultDto<AvailableBalanceTransactionDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DashboardPagingResultDto<AvailableBalanceTransactionDto>>> GetAvailableBalanceDetail([FromQuery] GetAvailableBalanceDetailParams parameters)
        {
            var response = await mediator.Send(new GetAvailableBalanceDetailQuery(parameters));

            return Ok(
                new BaseResponse<DashboardPagingResultDto<AvailableBalanceTransactionDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get available balance detail success",
                    response));
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
        /// 
        /// The response includes TotalProfitSum which is the sum of all TotalProfit values in the result set.
        /// </remarks>
        /// <param name="parameters">Query parameters for paging, filtering, and sorting. Includes: Page, Size, DoApplyPaging, SearchTerm, From, To.</param>
        /// <returns>
        /// A <see cref="BaseResponse{DashboardPagingResultDto{PendingBalanceOrderItemDto}}"/> containing paginated pending balance orders with profit sum.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        /// <response code="200">Pending balance details retrieved successfully</response>
        /// <response code="401">Unauthorized - User must be authenticated</response>
        /// <response code="403">Forbidden - Only GymOwner and FreelancePT can access</response>
        [HttpGet("pending-balance-detail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<DashboardPagingResultDto<PendingBalanceOrderItemDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DashboardPagingResultDto<PendingBalanceOrderItemDto>>> GetPendingBalanceDetail([FromQuery] GetPendingBalanceDetailParams parameters)
        {
            var response = await mediator.Send(new GetPendingBalanceDetailQuery(parameters));

            return Ok(
                new BaseResponse<DashboardPagingResultDto<PendingBalanceOrderItemDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get pending balance detail success",
                    response));
        }

        /// <summary>
        /// Retrieves detailed revenue information for all order items with pagination, search, and filtering support.
        /// </summary>
        /// <remarks>
        /// This endpoint returns all revenue order items regardless of distribution status (both distributed and pending).
        /// It provides comprehensive profit information including merchant profit, system commission, and distribution dates.
        /// 
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
        ///
        /// Each revenue item includes:
        /// - Order item details (ID, quantity, price, subtotal)
        /// - Profit information (merchant profit, system profit/commission, commission rate)
        /// - Coupon information (code, discount percentage, ID if applicable)
        /// - Course/Package details (ID, name)
        /// - Customer information (ID, full name)
        /// - Distribution dates (both planned and actual if distributed)
        /// 
        /// The response includes TotalProfitSum which is the sum of all TotalProfit values in the result set.
        /// </remarks>
        /// <param name="parameters">Query parameters for paging, filtering, and sorting. Includes: Page, Size, DoApplyPaging, SearchTerm, From, To.</param>
        /// <returns>
        /// A <see cref="BaseResponse{DashboardPagingResultDto{RevenueOrderItemDto}}"/> containing paginated revenue order items with profit sum.
        /// Returns HTTP 200 with the paginated result.
        /// </returns>
        /// <response code="200">Revenue details retrieved successfully</response>
        /// <response code="401">Unauthorized - User must be authenticated</response>
        /// <response code="403">Forbidden - Only GymOwner and FreelancePT can access</response>
        [HttpGet("revenue-detail")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<DashboardPagingResultDto<RevenueOrderItemDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DashboardPagingResultDto<RevenueOrderItemDto>>> GetRevenueDetail([FromQuery] GetRevenueDetailParams parameters)
        {
            var response = await mediator.Send(new GetRevenueDetailQuery(parameters));

            return Ok(
                new BaseResponse<DashboardPagingResultDto<RevenueOrderItemDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get revenue detail success",
                    response));
        }
    }
}