using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Contracts;
using FitBridge_Application.Features.Contracts.ConfirmContract;
using FitBridge_Application.Features.Contracts.CreateContract;
using FitBridge_Application.Features.Contracts.GetContract;
using FitBridge_Application.Features.Contracts.UpdateContract;
using FitBridge_Application.Specifications.Contracts.GetContract;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class ContractsController(IMediator mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateContract([FromBody] CreateContractCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<Guid>(StatusCodes.Status200OK.ToString(), "Contract created successfully", result));
    }

    /// <summary>
    /// Update contract files
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> UpdateContract([FromForm] UpdateContractCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<Guid>(StatusCodes.Status200OK.ToString(), "Contract updated successfully", result));
    }

    /// <summary>
    /// Admin confirm contract
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("confirm/{id}")]
    public async Task<IActionResult> ConfirmContract([FromRoute] Guid id)
    {
        var command = new ConfirmContractCommand { Id = id };
        var result = await mediator.Send(command);
        return Ok(new BaseResponse<Guid>(StatusCodes.Status200OK.ToString(), "Contract confirmed successfully", result));
    }

    /// <summary>
    /// Get contracts by CustomerId or get all contracts
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination, including:
    /// <list type="bullet">
    /// <item>
    /// <term>CustomerId</term>
    /// <description>The ID of the customer to filter by.</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetContracts([FromQuery] GetContractsParams query)
    {
        var result = await mediator.Send(new GetContractsQuery { Params = query });
        var pagination = ResultWithPagination(result.Items, result.Total, query.Page, query.Size);
        return Ok(new BaseResponse<Pagination<GetContractsDto>>(StatusCodes.Status200OK.ToString(), "Contracts retrieved successfully", pagination));
    }
}
