using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Features.Orders.CreateOrders;
using FitBridge_Application.Features.Orders.CreateShippingOrder;
using FitBridge_Application.Features.Orders.GetOrderByCustomerPurchasedId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class OrdersController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand orderDto)
    {
        var order = await _mediator.Send(orderDto);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Order created successfully", order));
    }

    [HttpGet("customer-purchased/{customerPurchasedId}")]
    public async Task<IActionResult> GetOrderByCustomerPurchasedId([FromRoute] Guid customerPurchasedId)
    {
        var order = await _mediator.Send(new GetOrderByCustomerPurchasedIdQuery { CustomerPurchasedId = customerPurchasedId });
        return Ok(new BaseResponse<OrderResponseDto>(StatusCodes.Status200OK.ToString(), "Order retrieved successfully", order));
    }

    [HttpPost("shipping")]
    public async Task<IActionResult> CreateShippingOrder([FromBody] CreateShippingOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<CreateShippingOrderResponseDto>(StatusCodes.Status200OK.ToString(), "Shipping order created successfully", result));
    }

    [HttpPost("shipping/webhook")]
    public async Task<IActionResult> ShippingCallbackWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var webhookData = await reader.ReadToEndAsync();
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Shipping webhook processed successfully", webhookData));
    }
}
