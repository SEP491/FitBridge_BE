using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Weights;
using FitBridge_Application.Features.Weights.GetAllWeights;
using FitBridge_Application.Features.Weights.CreateWeight;
using FitBridge_Application.Features.Weights.UpdateWeight;
using FitBridge_Application.Features.Weights.DeleteWeight;

namespace FitBridge_API.Controllers;

public class WeightsController(IMediator _mediator) : _BaseApiController
{
    /// <summary>
    /// Get all weights
    /// </summary>
    /// <returns>List of all weights</returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<List<WeightResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllWeights()
    {
        var result = await _mediator.Send(new GetAllWeightsQuery());
        return Ok(new BaseResponse<List<WeightResponseDto>>(StatusCodes.Status200OK.ToString(), "Weights retrieved successfully", result));
    }

    /// <summary>
    /// Create a new weight
    /// </summary>
    /// <param name="command">The weight details</param>
    /// <returns>The created weight ID</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/weights
    ///     {
    ///         "value": 5.0,
    ///         "unit": "lbs"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Weight created successfully</response>
    /// <response code="400">Weight with the same value and unit already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<CreateWeightResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWeight([FromBody] CreateWeightCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<CreateWeightResponseDto>(StatusCodes.Status200OK.ToString(), "Weight created successfully", result));
    }

    /// <summary>
    /// Update an existing weight
    /// </summary>
    /// <param name="id">The weight ID</param>
    /// <param name="command">The updated weight details</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/v1/weights/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///         "value": 10.0,
    ///         "unit": "kg"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Weight updated successfully</response>
    /// <response code="400">Weight with the same value and unit already exists</response>
    /// <response code="404">Weight not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWeight([FromRoute] Guid id, [FromBody] UpdateWeightCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Weight updated successfully", result));
    }

    /// <summary>
    /// Delete a weight (soft delete)
    /// </summary>
    /// <param name="id">The weight ID</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/v1/weights/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// This performs a soft delete, setting IsEnabled to false.
    /// The weight will no longer appear in the active weights list but remains in the database.
    /// </remarks>
    /// <response code="200">Weight deleted successfully</response>
    /// <response code="404">Weight not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWeight([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteWeightCommand { Id = id });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Weight deleted successfully", result));
    }
}
