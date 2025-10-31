using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Features.Transactions.GetAllGymOwnerTransaction;
using FitBridge_Application.Features.Transactions.GetCurrentUserTransactions;
using FitBridge_Application.Features.Transactions.GetGymOwnerTransactionById;
using FitBridge_Application.Features.Transactions.GetTransactionDetail;
using FitBridge_Application.Specifications.Transactions.GetAllGymOwnerTransaction;
using FitBridge_Application.Specifications.Transactions.GetCurrentUserTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller for managing transactions, including retrieval of transactions for all user types.
    /// </summary>
    [Authorize]
    public class TransactionsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves a paginated list of transactions for the current logged in user (Gym Owner or Freelance PT).
        /// </summary>
        /// <param name="parameters">Query parameters for filtering and pagination, including:
        /// <list type="bullet">
        /// <item>
        /// <term>Page</term>
        /// <description>The page number to retrieve.</description>
        /// </item>
        /// <item>
        /// <term>Size</term>
        /// <description>The number of items per page.</description>
        /// </item>
        /// <item>
        /// <term>SortBy</term>
        /// <description>The field to sort by (e.g., Amount, CreatedAt, Status).</description>
        /// </item>
        /// <item>
        /// <term>SortOrder</term>
        /// <description>The sort direction (asc or desc).</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>A paginated list of transactions for the current user.</returns>
        [HttpGet("current-user")]
        [Authorize(Roles = ProjectConstant.UserRoles.GymOwner + "," + ProjectConstant.UserRoles.FreelancePT + "," + ProjectConstant.UserRoles.Customer + "," + ProjectConstant.UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetTransactionsDto>>))]
        public async Task<ActionResult<Pagination<GetTransactionsDto>>> GetCurrentUserTransactions(
            [FromQuery] GetCurrentUserTransactionsParam parameters)
        {
            var response = await mediator.Send(new GetCurrentUserTransactionsQuery(parameters));

            var pagedResult = new Pagination<GetTransactionsDto>(
                response.Items,
                response.Total,
                parameters.Page,
                parameters.Size);

            return Ok(
                new BaseResponse<Pagination<GetTransactionsDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get current user transactions success",
                    pagedResult));
        }

        /// <summary>
        /// Retrieves the details of a specific transaction by its ID.
        /// </summary>
        /// <param name="transactionId">The unique identifier of the transaction.</param>
        /// <returns>The details of the specified transaction.</returns>
        [HttpGet("{transactionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<GetTransactionDetailDto>))]
        public async Task<ActionResult<GetTransactionDetailDto>> GetTransactionDetail([FromRoute] Guid transactionId)
        {
            var response = await mediator.Send(new GetTransactionDetailQuery { TransactionId = transactionId });

            return Ok(
                new BaseResponse<GetTransactionDetailDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Get transaction detail success",
                    response));
        }

        [HttpGet("gym-owner")]
        [Authorize(Roles = ProjectConstant.UserRoles.GymOwner)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetAllMerchantTransactionDto>>))]
        public async Task<ActionResult<Pagination<GetAllMerchantTransactionDto>>> GetAllGymOwnerTransactions([FromQuery] GetAllGymOwnerTransactionParams parameters)
        {
            var query = new GetAllGymOwnerTransactionQuery
            {
                Parameters = parameters
            };
            var response = await mediator.Send(query);
            var pagination = ResultWithPagination(response.Items, response.Total, parameters.Page, parameters.Size);
            return Ok(new BaseResponse<Pagination<GetAllMerchantTransactionDto>>(StatusCodes.Status200OK.ToString(), "Get all gym owner transactions success", pagination));
        }
        
        [HttpGet("gym-owner/{transactionId}")]
        [Authorize(Roles = ProjectConstant.UserRoles.GymOwner)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<MerchantTransactionDetailDto>))]
        public async Task<ActionResult<MerchantTransactionDetailDto>> GetGymOwnerTransactionById([FromRoute] Guid transactionId)
        {
            var response = await mediator.Send(new GetGymOwnerTransactionByIdCommand { TransactionId = transactionId });
            return Ok(new BaseResponse<MerchantTransactionDetailDto>(StatusCodes.Status200OK.ToString(), "Get gym owner transaction by id success", response));
        }
    }
}