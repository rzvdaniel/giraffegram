using GG.Auth.Dtos;
using GG.Auth.Models;
using GG.Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.EntityFrameworkCore.Models;

namespace GG.Auth.Controllers;

public class ApplicationController(AccountService accountService, ApplicationService applicationService) : AuthControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] ApplicationRegisterationDto applicationDto, CancellationToken cancellationToken)
    {
        var existingClient = await accountService.GetClientById(applicationDto.ClientId);

        if (existingClient != null)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var client = new ApplicationRegistration 
        { 
            ClientId = applicationDto.ClientId,
            ClientPassword = Guid.NewGuid().ToString(),
            DisplayName = applicationDto.DisplayName
        };

        await applicationService.Create(client, GetUserId(), cancellationToken);

        return Ok(client);
    }

    [HttpGet]
    public IAsyncEnumerable<OpenIddictEntityFrameworkCoreApplication> Get(CancellationToken cancellationToken)
    {
        return applicationService.List(GetUserId(), cancellationToken);
    }

    [HttpGet("{clientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationResultDto>> Get(string clientId, CancellationToken cancellationToken)
    {
        var application = await applicationService.Get(clientId, GetUserId(), cancellationToken);

        if (application is null)
        {
            return NotFound();
        }

        return Ok(application);
    }
}
