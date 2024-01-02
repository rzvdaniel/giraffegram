using GG.Auth.Dtos;
using GG.Auth.Models;
using GG.Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace GG.Auth.Controllers;

public class ClientController(AccountService accountService, ClientService clientService) : AppControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterClient ([FromBody] RegisterClientDto clientDto, CancellationToken cancellationToken)
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
    public IAsyncEnumerable<object> Get(CancellationToken cancellationToken)
    {
        return clientService.List(GetUserId(), cancellationToken);
    }
}
