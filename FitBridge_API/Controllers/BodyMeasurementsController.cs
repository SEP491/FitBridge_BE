using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.BodyMeasurements;
using FitBridge_Application.Features.BodyMeasurements.CreateBodyMeasurement;
using FitBridge_Application.Features.BodyMeasurements.DeleteBodyMeasurement;
using FitBridge_Application.Features.BodyMeasurements.GetBodyMeasurementRecordsByCustomerPurchasedId;
using FitBridge_Application.Features.BodyMeasurements.UpdateBodyMeasurement;
using FitBridge_Application.Specifications.BodyMeasurements.GetBodyMeasurementRecordsByCustomerPurchasedId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class BodyMeasurementsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateBodyMeasurement([FromBody] CreateBodyMeasurementCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Body measurement created successfully", result));
    }

    [HttpGet("{customerPurchasedId}")]
    public async Task<IActionResult> GetBodyMeasurements([FromRoute] Guid customerPurchasedId, [FromQuery] GetBodyMeasurementRecordsByCustomerPurchasedIdParams parameters)
    {
        var result = await _mediator.Send(new GetBodyMeasurementRecordsByCustomerPurchasedIdQuery { Params = parameters, CustomerPurchasedId = customerPurchasedId });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<BodyMeasurementRecordDto>>(StatusCodes.Status200OK.ToString(), "Body measurements retrieved successfully", pagination));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBodyMeasurement([FromRoute] Guid id, [FromBody] UpdateBodyMeasurementCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<BodyMeasurementRecordDto>(StatusCodes.Status200OK.ToString(), "Body measurement updated successfully", result));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBodyMeasurement([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteBodyMeasurementCommand { Id = id });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Body measurement deleted successfully", result));
    }
}