using GG.Auth.Dtos;
using GG.Auth.Entities;
using GG.Auth.Models;
using GG.Auth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Auth.Controllers;

public class AccountController(AccountService accountService) : AppControllerBase
{
    [AllowAnonymous]
    [HttpPost("registeruser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto model)
    {
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

        return Created();
    }

    [HttpPost("registerclient")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterClient ([FromBody] ClientRegisterDto clientDto, CancellationToken cancellationToken)
    {
        var existingClient = await accountService.GetClientById(clientDto.ClientId);

        if (existingClient != null)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var client = new RegisterClient 
        { 
            ClientId = clientDto.ClientId,
            ClientPassword = Guid.NewGuid().ToString(),
            DisplayName = clientDto.DisplayName
        };

        await accountService.CreateClient(client, cancellationToken);

        return Ok(client);
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
