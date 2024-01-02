using GG.Auth.Dtos;
using GG.Auth.Models;
using GG.Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace GG.Auth.Controllers;

public class ClientController(AccountService accountService, ClientService clientService) : AppControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create ([FromBody] ClientRegisterDto clientDto, CancellationToken cancellationToken)
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

        await clientService.CreateClient(client, GetUserId(), cancellationToken);

        return Ok(client);
    }

    [HttpGet]
    public IAsyncEnumerable<OpenIddictEntityFrameworkCoreApplication> Get(CancellationToken cancellationToken)
    {
        return clientService.List(GetUserId(), cancellationToken);
    }

    [HttpGet("{clientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientResultDto>> Get(string clientId, CancellationToken cancellationToken)
    {
        var application = await clientService.Get(clientId, GetUserId(), cancellationToken);

        if (application is null)
        {
            return NotFound();
        }

        return Ok(application);
    }
}
