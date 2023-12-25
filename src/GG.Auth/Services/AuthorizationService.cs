using GG.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Collections.Immutable;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Auth.Services;

public class AuthorizationService(
    AccountService userManagerService, 
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOpenIddictApplicationManager applicationManager)
{
    public async Task<ClaimsIdentity?> SignIn(string? userName, string? password)
    {
        var user = await userManagerService.GetUserByEmailOrUserName(userName);

        if (user == null)
        {
            return null;
        }

        // Validate the username/password parameters and ensure the account is not locked out.
        var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return null;
        }

        // Create the claims-based identity that will be used by OpenIddict to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Add the claims that will be persisted in the tokens.
        identity.SetClaim(Claims.Subject, await userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Role, (await userManager.GetRolesAsync(user)).ToImmutableArray());

        return identity;
    }

    public async Task<ClaimsIdentity?> SignIn(string? clientId)
    {
        // Note: the client credentials are automatically validated by OpenIddict:
        // if client_id or client_secret are invalid, this action won't be invoked.

        var application = await applicationManager.FindByClientIdAsync(clientId);

        if (application == null)
        {
            throw new InvalidOperationException("The application details cannot be found in the database.");
        }

        // Create the claims-based identity that will be used by OpenIddict to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Add the claims that will be persisted in the tokens (use the client_id as the subject identifier).
        identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application));
        identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));

        return identity;
    }
}