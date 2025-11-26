using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Features.Orders.CancelShippingOrder;
using FitBridge_Application.Features.Orders.CreateOrders;
using FitBridge_Application.Features.Orders.CreateShippingOrder;
using FitBridge_Application.Features.Orders.GetAllProductOrder;
using FitBridge_Application.Features.Orders.GetOrderByCustomerPurchasedId;
using FitBridge_Application.Features.Orders.GetShippingPrice;
using FitBridge_Application.Features.Orders.ProcessAhamoveWebhook;
using FitBridge_Application.Features.Orders.UpdateOrderStatus;
using FitBridge_Application.Specifications.Orders.GetAllProductOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// Create a shipping order for a product order
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("shipping")]
    public async Task<IActionResult> CreateShippingOrder([FromBody] CreateShippingOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<CreateShippingOrderResponseDto>(StatusCodes.Status200OK.ToString(), "Shipping order created successfully", result));
    }
    /// <summary>
    /// API for ahamove to callback when there is a change in the shipping order status
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("shipping/webhook")]
    public async Task<IActionResult> ShippingCallbackWebhook()
    {
        var reader = new StreamReader(Request.Body);
        var webhookPayload = await reader.ReadToEndAsync();

        // Use StreamReader to read the raw body since the data comes as a nested JSON string
        // using var reader = new StreamReader(Request.Body);
        // var webhookPayload = await reader.ReadToEndAsync();

        var command = new ProcessAhamoveWebhookCommand
        {
            WebhookPayload = webhookPayload
        };

        await _mediator.Send(command);

        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Shipping webhook processed successfully", webhookPayload));
    }
    /// <summary>
    /// Update the status of an order
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("status/{orderId}")]
    public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId, [FromBody] UpdateOrderStatusCommand command)
    {
        command.OrderId = orderId;
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<OrderStatusResponseDto>(StatusCodes.Status200OK.ToString(), "Order status updated successfully", result));
    }

    /// <summary>
    /// Cancel a shipping order
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("shipping/cancel/{orderId}")]
    public async Task<IActionResult> CancelShippingOrder([FromRoute] Guid orderId, [FromBody] CancelShippingOrderCommand command)
    {
        command.OrderId = orderId;
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Shipping order canceled successfully", result));
    }

    /// <summary>
    /// Estimate the shipping price for a product order
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("shipping/price-estimate")]
    public async Task<IActionResult> GetShippingPrice([FromBody] GetShippingPriceCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<ShippingEstimateDto>(StatusCodes.Status200OK.ToString(), "Shipping price retrieved successfully", result));
    }
    
    /// <summary>
    /// Get all product orders
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("product")]
    public async Task<IActionResult> GetAllProductOrders([FromQuery] GetAllProductOrdersParams parameters)
    {
        var result = await _mediator.Send(new GetAllProductOrdersQuery { Params = parameters });
        var pagination = ResultWithPagination(result.ProductOrders.Items, result.ProductOrders.Total, parameters.Page, parameters.Size);
        var response = new
        {
            result.SummaryProductOrder,
            ProductOrders = pagination
        };
        return Ok(new BaseResponse<object>(StatusCodes.Status200OK.ToString(), "Orders retrieved successfully", response));
    }
}
