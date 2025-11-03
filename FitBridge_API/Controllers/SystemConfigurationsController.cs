using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Features.SystemConfigurations;
using FitBridge_Application.Dtos.SystemConfigs;
using FitBridge_Domain.Entities.Systems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class SystemConfigurationsController(IMediator _mediator) : _BaseApiController
{
    /// <summary>
    /// Create a new system configuration
    /// The data type of the system configuration, 1: String, 2: Int, 3: Decimal, 4: Double, 5: Boolean
    /// The value of the system configuration, with decimal and double value use , as decimal separator don't use
    /// </summary>
    /// <returns>The created system configuration</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSystemConfiguration([FromBody] CreateSystemConfigurationCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "System configuration created successfully", result));
    }

    [HttpGet("{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSystemConfiguration([FromRoute] string key)
    {
        var result = await _mediator.Send(new GetSystemConfigQuery { Key = key });
        return Ok(new BaseResponse<object>(StatusCodes.Status200OK.ToString(), "System configuration retrieved successfully", result));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSystemConfigurations()
    {
        var result = await _mediator.Send(new GetAllSystemConfigurationsQuery());
        return Ok(new BaseResponse<List<SystemConfigurationDto>>(StatusCodes.Status200OK.ToString(), "System configurations retrieved successfully", result));
    }
}
