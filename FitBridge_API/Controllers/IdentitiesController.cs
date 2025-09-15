using System;
using FitBridge_API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Features.Identities.Registers;
using FitBridge_Application.Features.Identities.Registers.RegisterAccounts;

namespace FitBridge_API.Controllers;

public class IdentitiesController(IMediator _mediator, UserManager<ApplicationUser> _userManager) : _BaseApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(int.Parse(result.Status), result);
    }

    [AllowAnonymous]
    [HttpPost("register-accounts")]
    public async Task<IActionResult> RegisterAccounts([FromBody] RegisterAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(int.Parse(result.Status), result);
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest(new { status = "400", message = "Invalid email address", data = user.Id });
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return BadRequest(new { status = "400", message = "Email confirmation failed", data = user.Id });
        }

        return Ok(new { status = "200", message = "Email confirmed successfully", data = user.Id });
    }
    
    
}
