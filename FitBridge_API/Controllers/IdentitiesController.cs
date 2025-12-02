using System;
using FitBridge_API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Features.Identities.Registers.RegisterAccounts;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Identities;
using FitBridge_Application.Features.Identities.Login;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Dtos.Tokens;
using FitBridge_Application.Features.Identities.Token;
using FitBridge_Application.Features.Identities.Registers.RegisterGymPT;
using System.Security.Claims;
using FitBridge_Application.Features.Identities.Registers.RegisterCustomer;
using FitBridge_Application.Features.Accounts.UpdateLoginInfo;
using FitBridge_Application.Features.Accounts.UpdatePassword;

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
    public async Task<IActionResult> RegisterAccounts([FromForm] RegisterAccountCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "User created successfully", result.UserId.ToString()));
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), ex.Message, ex.InnerException?.Message));
        }
    }

    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _applicationUserService.GetUserByEmailAsync(email, false);
        if (user == null)
        {
            return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), "Invalid email address", null));
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
        response = await _mediator.Send(command);
        //return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), ex.Message, null));
        return Ok(new BaseResponse<LoginResponseDTO>(StatusCodes.Status200OK.ToString(), "Login successful", response));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenCommand refreshToken)
    {
        try
        {
            var result = await _mediator.Send(refreshToken);
            return Ok(new BaseResponse<RefreshTokenResponse>(StatusCodes.Status200OK.ToString(), "Token refreshed successfully", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse<string>(
            StatusCodes.Status400BadRequest.ToString(),
            ex.InnerException.Message,
            null));
        }
    }

    [HttpPost("register-gym-pt")]
    public async Task<IActionResult> RegisterGymPt([FromBody] RegisterGymPtCommand command)
    {
        try
        {
            command.GymOwnerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (command.GymOwnerId == "")
            {
                return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), "Gym owner not found", null));
            }
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<CreateNewPTResponse>(StatusCodes.Status200OK.ToString(), "Gym Pt created successfully", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), ex.Message, ex.InnerException?.Message));
        }
    }

    /// <summary>
    /// Update email and phone number of the authenticated user
    /// </summary>
    /// <param name="command">The command containing the new email and phone number.</param>
    /// <returns>A boolean indicating whether the login info was updated successfully.</returns>
    [HttpPut("update-login-info")]
    public async Task<IActionResult> UpdateLoginInfo([FromBody] UpdateLoginInfoCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Login info updated successfully", result));
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Password updated successfully", result));
    }
}