using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Features.Payments.CreatePaymentLink;
using FitBridge_Application.Features.Payments.PaymentCallbackWebhook;
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

        return BadRequest(new BaseResponse<bool>(StatusCodes.Status400BadRequest.ToString(), "Failed to process webhook", result));
    }
}
