using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Features.Orders.CreateOrders;
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
    }
