﻿using GG.Auth.Enums;
using GG.Auth.Models;
using GG.Auth.Services;
using GG.Core.Models;
using GG.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Api.Controllers;

public class AccountController(AccountService accountService, AppEmailService appEmailService, AppConfigService appConfigService) : AppControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Create([FromBody] UserRegistration userRegistration, CancellationToken cancellationToken)
    {
        if (!appConfigService.AppConfig.AllowUserRegistration)
        {
            return NotFound();
        }

        var existingUser = await accountService.GetUserByEmailOrUserName(userRegistration.Email);

        if (existingUser != null)
        {
            return Conflict();
        }

        var result = await accountService.CreateUser(userRegistration, cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest();
        }

        var user = await accountService.GetUserByEmailOrUserName(userRegistration.Email);

        if (user == null)
        {
            return BadRequest();
        }

        await accountService.AddUserToRole(user.Id, UserRoles.User);

        var userDetails = new UserDetails { Email = userRegistration.Email, Name = userRegistration.Name };
        await appEmailService.SendRegistrationEmail(userDetails, cancellationToken);

        return Created();
    }

    [HttpGet("info"), HttpPost("info"), Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Info()
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

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword, CancellationToken cancellationToken)
    {
        var passwordResetToken = await accountService.GetPasswordResetToken(forgotPassword, cancellationToken);

        if (passwordResetToken == null)
        {
            return BadRequest();
        }

        var user = await accountService.GetUserByEmailOrUserName(forgotPassword.Email);

        if (user == null)
        {
            return BadRequest();
        }

        var passwordResetDto = new UserForgotPassword
        {
            Email = user.Email!,
            Name = user.Name,
            Token = passwordResetToken
        };

        await appEmailService.SendResetPasswordEmail(passwordResetDto, cancellationToken);

        return Ok(new { message = "Please check your email for password reset instructions" });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> ResetPassword(ResetPassword resetPassword, CancellationToken cancellationToken)
    {
        var user = await accountService.GetUserByEmailOrUserName(resetPassword.Email);

        if (user == null)
        {
            return BadRequest();
        }

        var result = await accountService.ResetPassword(user, resetPassword.Token, resetPassword.Password, cancellationToken);

        return result.Succeeded ? 
            Ok(new { message = "Password reset successfully" }) : 
            BadRequest(result.Errors.Select(x => x.Description));
    }
}
