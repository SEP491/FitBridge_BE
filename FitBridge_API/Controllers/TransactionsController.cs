using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Application.Features.Transactions.GetFreelancePtTransactions;
using FitBridge_Application.Features.Transactions.GetTransactionDetail;
using FitBridge_Application.Specifications.Transactions.GetTransactionByPtId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller for managing transactions, including retrieval of transactions for freelance PTs.
    /// </summary>
    [Authorize]
    public class TransactionsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Retrieves a paginated list of transactions for a specific freelance PT.
        /// </summary>
        /// <param name="ptId">The unique identifier of the freelance PT.</param>
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
        /// <returns>A paginated list of transactions for the specified freelance PT.</returns>
        [HttpGet("freelance-pt/{ptId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetTransactionsDto>>))]
        public async Task<ActionResult<Pagination<GetTransactionsDto>>> GetFreelancePtTransactions([FromQuery] GetTransactionByPtIdParam parameters)
        {
            var response = await mediator.Send(new GetFreelancePtTransactionsQuery(parameters));

            var pagedResult = new Pagination<GetTransactionsDto>(
                response.Items,
                response.Total,
                parameters.Page,
                parameters.Size);

            return Ok(
                new BaseResponse<Pagination<GetTransactionsDto>>(
                    StatusCodes.Status200OK.ToString(),
                    "Get freelance PT transactions success",
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
    }
}