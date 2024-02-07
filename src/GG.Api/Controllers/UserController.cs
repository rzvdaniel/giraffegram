﻿using GG.Auth.Dtos;
using GG.Auth.Entities;
using GG.Auth.Services;
using GG.Core.Dto;
using GG.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Api.Controllers;

public class UserController(AccountService accountService, AppEmailService appEmailService) : AuthControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] UserRegisterDto model, CancellationToken cancellationToken)
    {
        var existingUser = await accountService.GetUserByEmailOrUserName(model.Email);

        if (existingUser != null)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var newUser = new User 
        { 
            UserName = model.Email,
            Name = model.Name,
            Email = model.Email 
        };

        var result = await accountService.CreateUser(newUser, model.Password);

        if (!result.Succeeded)
        {
            AddErrors(result);
        }
 
        var newUserDto = new NewUserDto { Email = model.Email, Name = model.Name };
        await appEmailService.SendRegistrationEmail(newUserDto, cancellationToken);

        return Created();
    }

    [HttpGet("userinfo"), HttpPost("userinfo"), Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UserInfo()
    {
        var userId = User.GetClaim(Claims.Subject) ?? 
            throw new InvalidOperationException("User information cannot be retrieved from the request.");

        var user = await accountService.GetUserById(userId);

        if (user == null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that no longer exists."
                }));
        }

        var claims = await accountService.GetUserClaims(user);

        return Ok(claims);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }
    }
}
