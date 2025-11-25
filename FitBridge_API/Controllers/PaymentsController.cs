using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Features.Payments.AppleWebhook;
using FitBridge_Application.Features.Payments.ApproveWithdrawalRequest;
using FitBridge_Application.Features.Payments.CancelPaymentCommand;
using FitBridge_Application.Features.Payments.ConfirmWithdrawalRequest;
using FitBridge_Application.Features.Payments.CreatePaymentLink;
using FitBridge_Application.Features.Payments.CreateRequestPayment;
using FitBridge_Application.Features.Payments.GetAllWithdrawalRequests;
using FitBridge_Application.Features.Payments.GetPaymentInfor;
using FitBridge_Application.Features.Payments.PaymentCallbackWebhook;
using FitBridge_Application.Features.Payments.RejectWithdrawalRequest;
using FitBridge_Application.Features.Payments.RePaidOrder;
using FitBridge_Application.Features.Refund.RefundItem;
using FitBridge_Application.Specifications.Payments.GetAllWithdrawalRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class PaymentsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost("payment-link")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreatePaymentLinkCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<PaymentResponseDto>(StatusCodes.Status200OK.ToString(), "Payment link created successfully", result));
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> PaymentCallbackWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var webhookData = await reader.ReadToEndAsync();
        var spec = new PaymentCallbackWebhookCommand { WebhookData = webhookData };
        var result = await _mediator.Send(spec);

        if (result)
        {
            return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Webhook processed successfully", result));
        }

        //return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Webhook processed successfully", true));

        return BadRequest(new BaseResponse<bool>(StatusCodes.Status400BadRequest.ToString(), "Failed to process webhook", result));
    }

    [HttpPost("apple-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> AppleWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var webhookData = await reader.ReadToEndAsync();
        var spec = new AppleWebhookCommand { WebhookData = webhookData };
        var result = await _mediator.Send(spec);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Apple webhook processed successfully", result));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentInfo(string id)
    {
        var result = await _mediator.Send(new GetPaymentInfoCommand { Id = id });
        return Ok(new BaseResponse<PaymentInfoResponseDto>(StatusCodes.Status200OK.ToString(), "Payment information retrieved successfully", result));
    }

    [HttpPost("cancel")]
    [Authorize]
    public async Task<IActionResult> CancelPayment([FromBody] CancelPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Payment cancelled successfully", result));
    }

    /// <summary>
    /// Requests a withdrawal payment for the authenticated user.
    /// </summary>
    /// <param name="command">The withdrawal request details, including:
    /// <list type="bullet">
    /// <item>
    /// <term>Amount</term>
    /// <description>The amount to withdraw.</description>
    /// </item>
    /// <item>
    /// <term>BankName</term>
    /// <description>The name of the bank for withdrawal.</description>
    /// </item>
    /// <item>
    /// <term>AccountName</term>
    /// <description>The bank account holder's name.</description>
    /// </item>
    /// <item>
    /// <term>AccountNumber</term>
    /// <description>The bank account number.</description>
    /// </item>
    /// <item>
    /// <term>Reason</term>
    /// <description>The reason for the withdrawal request.</description>
    /// </item>
    /// <item>
    /// <term>Note</term>
    /// <description>Additional notes for the withdrawal (optional).</description>
    /// </item>
    /// <item>
    /// <term>ImageUrl</term>
    /// <description>URL of any supporting documentation image (optional).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>The created withdrawal request with the request ID.</returns>
    /// <response code="201">Returns the newly created withdrawal request.</response>
    /// <response code="400">If a withdrawal request already exists for the user.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpPost("request-withdrawal")]
    [Authorize(Roles = ProjectConstant.UserRoles.FreelancePT + "," + ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponse<RequestPaymentResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RequestPayment([FromBody] CreateRequestPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return Created(
            nameof(RequestPayment),
            new BaseResponse<RequestPaymentResponseDto>(
                StatusCodes.Status201Created.ToString(),
                "Withdrawal request submitted successfully",
                result));
    }

    /// <summary>
    /// Gets all withdrawal requests.
    /// Admin users can view all requests, while GymOwner and FreelancePT users can only view their own requests.
    /// </summary>
    /// <param name="parameters">Query parameters for pagination and filtering.</param>
    /// <returns>A paginated list of withdrawal requests.</returns>
    /// <response code="200">Returns the list of withdrawal requests.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpGet("withdrawal-requests")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin + "," + ProjectConstant.UserRoles.FreelancePT + "," + ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<PagingResultDto<GetWithdrawalRequestResponseDto>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllWithdrawalRequests([FromQuery] GetAllWithdrawalRequestsParams parameters)
    {
        var query = new GetAllWithdrawalRequestsQuery { Params = parameters };
        var result = await _mediator.Send(query);
        return Ok(new BaseResponse<PagingResultDto<GetWithdrawalRequestResponseDto>>(
            StatusCodes.Status200OK.ToString(),
            "Withdrawal requests retrieved successfully",
            result));
    }

    /// <summary>
    /// Approves a withdrawal request by admin.
    /// Updates the withdrawal request status to AdminApproved, creates a transaction record with the proof image URL,
    /// and sends notification to the user upon approval.
    /// </summary>
    /// <param name="withdrawalRequestId">The ID of the withdrawal request to approve.</param>
    /// <param name="command">The approval details including the proof image URL.</param>
    /// <returns>Success response if the approval is successful.</returns>
    /// <response code="200">Returns success if the withdrawal request is approved.</response>
    /// <response code="400">If the request is not in pending status or validation fails.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not an admin.</response>
    /// <response code="404">If the withdrawal request is not found.</response>
    [HttpPut("withdrawal-requests/{withdrawalRequestId}/approve")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveWithdrawalRequest(
        [FromRoute] Guid withdrawalRequestId,
        [FromBody] ApproveWithdrawalRequestCommand command)
    {
        command.WithdrawalRequestId = withdrawalRequestId;
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            "Withdrawal request approved successfully",
            Empty));
    }

    /// <summary>
    /// Confirms a withdrawal request by the user.
    /// User can confirm their own withdrawal request that has been approved by admin.
    /// This action marks the request as resolved.
    /// </summary>
    /// <param name="withdrawalRequestId">The ID of the withdrawal request to confirm.</param>
    /// <returns>Success response if the confirmation is successful.</returns>
    /// <response code="200">Returns success if the withdrawal request is confirmed by user.</response>
    /// <response code="400">If the request is not in AdminApproved status.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not authorized to confirm this request.</response>
    /// <response code="404">If the withdrawal request is not found.</response>
    [HttpPut("withdrawal-requests/{withdrawalRequestId}/confirm")]
    [Authorize(Roles = ProjectConstant.UserRoles.FreelancePT + "," + ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmWithdrawalRequest([FromRoute] Guid withdrawalRequestId)
    {
        var command = new ConfirmWithdrawalRequestCommand { WithdrawalRequestId = withdrawalRequestId };
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            "Withdrawal request confirmed successfully",
            Empty));
    }

    /// <summary>
    /// Rejects a withdrawal request.
    /// Admin can reject any pending withdrawal request.
    /// Users can reject (disapprove) their own pending withdrawal request.
    /// Sends notification to admins or user upon rejection.
    /// </summary>
    /// <param name="withdrawalRequestId">The ID of the withdrawal request to reject.</param>
    /// <param name="command">The rejection details including the reason.</param>
    /// <returns>Success response if the rejection is successful.</returns>
    /// <response code="200">Returns success if the withdrawal request is rejected.</response>
    /// <response code="400">If the request is not in pending status or validation fails.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not authorized to reject this request.</response>
    /// <response code="404">If the withdrawal request is not found.</response>
    [HttpPut("withdrawal-requests/{withdrawalRequestId}/reject")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin + "," + ProjectConstant.UserRoles.FreelancePT + "," + ProjectConstant.UserRoles.GymOwner)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectWithdrawalRequest(
        [FromRoute] Guid withdrawalRequestId,
        [FromBody] RejectWithdrawalRequestCommand command)
    {
        command.WithdrawalRequestId = withdrawalRequestId;
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            "Withdrawal request rejected successfully",
            Empty));
    }

    /// <summary>
    /// Refunds an order item.
    /// Admin can refund an order item by its ID.
    /// If a profit distribution job is scheduled for the item, it will be cancelled.
    /// Otherwise, the refund amount will be deducted from the seller's wallet.
    /// Sends notification to the customer upon successful refund.
    /// </summary>
    /// <param name="orderItemId">The ID of the order item to refund.</param>
    /// <returns>Success response if the refund is successful.</returns>
    /// <response code="200">Returns success if the order item is refunded.</response>
    /// <response code="400">If the order item has already been refunded or validation fails.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not an admin.</response>
    /// <response code="404">If the order item or wallet is not found.</response>
    [HttpPost("refund/{orderItemId}")]
    [Authorize(Roles = ProjectConstant.UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefundItem([FromRoute] Guid orderItemId)
    {
        var command = new RefundItemCommand { OrderItemId = orderItemId };
        await _mediator.Send(command);
        return Ok(new BaseResponse<EmptyResult>(
            StatusCodes.Status200OK.ToString(),
            "Order item refunded successfully",
            Empty));
    }
    /// <summary>
    /// Check an order checkout url and create new ones with new transaction if old one is expired
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("re-paid-order")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<EmptyResult>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RePaidOrder([FromBody] RePaidOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Order re-paid successfully", result));
    }
}