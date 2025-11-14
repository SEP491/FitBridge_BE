using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Flavours;
using FitBridge_Application.Features.Flavours.GetAllFlavours;
using FitBridge_Application.Features.Flavours.CreateFlavour;
using FitBridge_Application.Features.Flavours.UpdateFlavour;
using FitBridge_Application.Features.Flavours.DeleteFlavour;

namespace FitBridge_API.Controllers;

public class FlavoursController(IMediator _mediator) : _BaseApiController
{
    /// <summary>
    /// Get all flavours
    /// </summary>
    /// <returns>List of all flavours</returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<List<FlavourResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFlavours()
    {
        var result = await _mediator.Send(new GetAllFlavoursQuery());
        return Ok(new BaseResponse<List<FlavourResponseDto>>(StatusCodes.Status200OK.ToString(), "Flavours retrieved successfully", result));
    }

    /// <summary>
    /// Create a new flavour
    /// </summary>
    /// <param name="command">The flavour details</param>
    /// <returns>The created flavour ID</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/flavours
    ///     {
    ///         "name": "Chocolate"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Flavour created successfully</response>
    /// <response code="400">Flavour with the same name already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<CreateFlavourResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFlavour([FromBody] CreateFlavourCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<CreateFlavourResponseDto>(StatusCodes.Status200OK.ToString(), "Flavour created successfully", result));
    }

    /// <summary>
    /// Update an existing flavour
    /// </summary>
    /// <param name="id">The flavour ID</param>
    /// <param name="command">The updated flavour details</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/v1/flavours/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///         "name": "Dark Chocolate"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Flavour updated successfully</response>
    /// <response code="400">Flavour with the same name already exists</response>
    /// <response code="404">Flavour not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFlavour([FromRoute] Guid id, [FromBody] UpdateFlavourCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Flavour updated successfully", result));
    }

    /// <summary>
    /// Delete a flavour (soft delete)
    /// </summary>
    /// <param name="id">The flavour ID</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/v1/flavours/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// This performs a soft delete, setting IsEnabled to false.
    /// The flavour will no longer appear in the active flavours list but remains in the database.
    /// </remarks>
    /// <response code="200">Flavour deleted successfully</response>
    /// <response code="404">Flavour not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFlavour([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteFlavourCommand { Id = id });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Flavour deleted successfully", result));
    }
}