using GG.Core.Dto;
using GG.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class SetupController(SetupService setupService) : AppControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> Create(UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
    {
        var isSetupComplete = await setupService.IsSetupComplete();

        if (isSetupComplete)
            return NotFound();

       await setupService.Setup(userRegisterDto, cancellationToken);

        return Created();
    }
}
