using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Features.Payments.CancelPaymentCommand;
using FitBridge_Application.Features.Payments.CreatePaymentLink;
using FitBridge_Application.Features.Payments.GetPaymentInfor;
using FitBridge_Application.Features.Payments.PaymentCallbackWebhook;
using FitBridge_Application.Features.Payments.RequestPayment;
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
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BaseResponse<RequestPaymentResponseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RequestPayment([FromBody] RequestPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return Created(
            nameof(RequestPayment),
            new BaseResponse<RequestPaymentResponseDto>(
                StatusCodes.Status201Created.ToString(),
                "Withdrawal request submitted successfully",
                result));
    }
}