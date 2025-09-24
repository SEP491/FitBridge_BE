using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Features.Payments.CreatePaymentLink;
using MediatR;
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
}
