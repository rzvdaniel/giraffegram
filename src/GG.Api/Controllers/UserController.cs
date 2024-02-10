using GG.Auth.Models;
using GG.Auth.Services;
using GG.Core.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Api.Controllers;

public class UserController(AccountService accountService) : AuthControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
    {
        var existingUser = await accountService.GetUserByEmailOrUserName(userRegisterDto.Email);

        if (existingUser != null)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var result = await accountService.CreateUser(userRegisterDto, cancellationToken);

        if (!result.Succeeded)
        {
            AddErrors(result);
        }
 
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

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword, CancellationToken cancellationToken)
    {
        await accountService.SendResetPasswordEmail(forgotPassword, cancellationToken);

        return Ok();
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }
    }
}
