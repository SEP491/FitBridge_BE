using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Messaging;
using FitBridge_Application.Features.Messaging.GetConversations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

/// <summary>
/// Controller for managing messaging and conversations.
/// </summary>
[Authorize]
public class MessagingController(IMediator _mediator) : _BaseApiController
{
    /// <summary>
    /// Retrieves a paginated list of conversations for the authenticated user.
    /// </summary>
    /// <param name="query">Query parameters for pagination including:
    /// <list type="bullet">
    /// <item>
    /// <term>PageNumber</term>
    /// <description>The page number to retrieve (default: 1).</description>
    /// </item>
    /// <item>
    /// <term>PageSize</term>
    /// <description>The number of items per page (default: 20).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>A paginated list of conversations with their details.</returns>
    /// <remarks>
    /// Returns conversation information including:
    /// - Basic info (ID, IsGroup, CustomTitle, UpdatedAt)
    /// - Last message details (Content, Type, MediaType, Sender)
    /// - Read status for the current user
    /// - Conversation members with their roles
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/messaging/conversations?pageNumber=1&amp;pageSize=20
    ///
    /// </remarks>
    /// <response code="200">Conversations retrieved successfully</response>
    /// <response code="401">Unauthorized - User not authenticated</response>
    [HttpGet("conversations")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetConversationsDto>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetConversations([FromQuery] GetConversationsQuery query)
    {
        var response = await _mediator.Send(query);
        var pagination = ResultWithPagination(response.Items, response.Total, query.PageNumber, query.PageSize);
        return Ok(new BaseResponse<Pagination<GetConversationsDto>>(
            StatusCodes.Status200OK.ToString(),
            "Conversations retrieved successfully",
            pagination));
    }
}