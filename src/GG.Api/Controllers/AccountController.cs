using GG.Auth.Entities;
using GG.Auth.Models;
using GG.Auth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Api.Controllers;

public class AccountController(AccountService accountService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingUser = await accountService.GetUserByEmailOrUserName(model.Email);

        if (existingUser != null)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email };

        var result = await accountService.CreateUser(newUser, model.Password);

        if (!result.Succeeded)
        {
            AddErrors(result);
        }

        return Ok();
    }

    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("userinfo"), HttpPost("userinfo"), Produces("application/json")]
    public async Task<IActionResult> UserInfo()
    {
        var userId = User.GetClaim(Claims.Subject) 
            ?? throw new InvalidOperationException("User information cannot be retrieved from the request.");

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

        var claims = await accountService.GetUserClaims(user, User);

        return Ok(claims);
    }
}
