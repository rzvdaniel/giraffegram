using GG.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class SetupController(SetupService setupService) : AppControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> Create(UserRegistration userRegisterDto, CancellationToken cancellationToken)
    {
        var isSetupComplete = await setupService.IsSetupComplete();

        if (isSetupComplete)
            return NotFound();

        await setupService.Setup(userRegisterDto, cancellationToken);

        return Created();
    }
}
