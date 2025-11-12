using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Flavours;
using FitBridge_Application.Features.Flavours.GetAllFlavours;
namespace FitBridge_API.Controllers;

public class FlavoursController(IMediator _mediator) : _BaseApiController   
{
    [HttpGet]
    public async Task<IActionResult> GetAllFlavours()
    {
        var result = await _mediator.Send(new GetAllFlavoursQuery());
        return Ok(new BaseResponse<List<FlavourResponseDto>>(StatusCodes.Status200OK.ToString(), "Flavours retrieved successfully", result));
    }   
}
