using GG.Portal.Services.Account;
using GG.Portal.Services.Setup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GG.Portal.Controllers;

public class SetupController(SetupService setupService) : AppControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> Create(UserRegistrationCommand userRegisterDto, CancellationToken cancellationToken)
    {
        var isSetupComplete = await setupService.IsSetupComplete();

        if (isSetupComplete)
            return Conflict();

        await setupService.Setup(userRegisterDto, cancellationToken);

        return Created();
    }
}
