using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Features.Orders.CreateOrders;
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
}
