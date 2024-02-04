using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace GG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public abstract class AuthControllerBase : ControllerBase
{    
    protected Guid GetUserId()
    {
        var userIdString = User.GetClaim(Claims.Subject) ??
            throw new InvalidOperationException("User information cannot be retrieved from the request.");

        var success = Guid.TryParse(userIdString, out Guid userId);

        if (!success)
            throw new InvalidOperationException("User id cannot be retrieved from the request.");

        return userId;
    }
}