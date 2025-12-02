using System;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FitBridge_Application.Features.Addresses.CreateAddress;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Application.Features.Addresses.GetAllCustomerAddress;
using FitBridge_Application.Features.Addresses.GetAddressById;
using FitBridge_Application.Features.Addresses.DeleteAddress;
using FitBridge_Application.Features.Addresses.UpdateAddress;
using FitBridge_Application.Features.Addresses.GetAllShopAddress;
using FitBridge_Application.Specifications.Addresses.GetAllShopAddress;

namespace FitBridge_API.Controllers;

public class AddressesController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Address created successfully", result));
    }
    /// <summary>
    /// Get all addresses for a customer
    /// </summary>
    /// <returns></returns>
    [HttpGet("customer")]
    public async Task<IActionResult> GetAllByCustomer()
    {
        var result = await _mediator.Send(new GetAllCustomerAddressesQuery());
        return Ok(new BaseResponse<List<AddressResponseDto>>(StatusCodes.Status200OK.ToString(), "Addresses retrieved successfully", result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var result = await _mediator.Send(new GetAddressByIdQuery(Guid.Parse(id)));
        return Ok(new BaseResponse<AddressResponseDto>(StatusCodes.Status200OK.ToString(), "Address retrieved successfully", result));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress([FromRoute] string id)
    {
        var result = await _mediator.Send(new DeleteAddressCommand(Guid.Parse(id)));
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Address deleted successfully", result));
    }

    /// <summary>
    /// Update an address, pass IsShopDefaultAddress to switch default address to this address, cannot be false
    /// </summary>
    /// <param name="IsShopDefaultAddress">True to switch default address to this address, cannot be false</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAddress([FromRoute] string id, [FromBody] UpdateAddressCommand command)
    {
        command.Id = Guid.Parse(id);
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<AddressResponseDto>(StatusCodes.Status200OK.ToString(), "Address updated successfully", result));
    }

    /// <summary>
    /// Get all shop addresses of all admin accounts
    /// </summary>
    /// <returns></returns>
    [HttpGet("admin/shop-address")]
    public async Task<IActionResult> GetAllShopAddress([FromQuery] GetAllShopAddressParams parameters)
    {
        var result = await _mediator.Send(new GetAllShopAddressQuery(parameters));
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<AddressResponseDto>>(StatusCodes.Status200OK.ToString(), "Shop address retrieved successfully", pagination));
    }
}
