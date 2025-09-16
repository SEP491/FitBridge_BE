using System;
using FitBridge_API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Features.Identities.Registers;
using FitBridge_Application.Features.Identities.Registers.RegisterAccounts;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Identities;
using FitBridge_Application.Features.Identities.Login;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_API.Controllers;

public class IdentitiesController(IMediator _mediator, IApplicationUserService _applicationUserService) : _BaseApiController
{
    [HttpPost("register-customer")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "User created successfully", result.UserId.ToString()));
    }

    [AllowAnonymous]
    [HttpPost("register-other-accounts")]
    public async Task<IActionResult> RegisterAccounts([FromBody] RegisterAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "User created successfully", result.UserId.ToString()));
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _applicationUserService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), "Invalid email address", user.Id.ToString()));
        }

        var result = await _applicationUserService.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), "Email confirmation failed", user.Id.ToString()));
        }

        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Email confirmed successfully", user.Id.ToString()));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginUserCommand command)
    {
        LoginResponseDTO response;
        try
        {
            response = await _mediator.Send(command);
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), ex.Message, null));
        }
        return Ok(new BaseResponse<LoginResponseDTO>(StatusCodes.Status200OK.ToString(), "Login successful", response));
    }
}
