using System;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Features.Uploads;

namespace FitBridge_API.Controllers;

public class UploadsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] UploadFileCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "File uploaded successfully", result));
    }
}
