using GG.Core.Dto;
using GG.Core.Entities;
using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class EmailAccountController(EmailAccountService emailAccountService) : AppControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create(EmailAccountAddDto emailHost, CancellationToken cancellationToken)
    {
        var emailHostExists = await emailAccountService.Exists(emailHost.Name, GetUserId(), cancellationToken);

        if (emailHostExists)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var id = await emailAccountService.Create(emailHost, GetUserId(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, emailHost);
    }

    [HttpGet]
    public async Task<IEnumerable<EmailAccount>> Get(CancellationToken cancellationToken)
    {
        return await emailAccountService.List(GetUserId(), cancellationToken);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmailAccountGetDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var emailAccount = await emailAccountService.Get(id, GetUserId(), cancellationToken);

        if (emailAccount is null)
        {
            return NotFound();
        }

        var emailAccountDto = new EmailAccountGetDto
        {
            Id = emailAccount.Id,
            UserName = emailAccount.UserName,
            Name = emailAccount.Name,
            Host = emailAccount.Host,
            Port = emailAccount.Port,
            UseSsl = emailAccount.UseSsl,
            CreatedAt = emailAccount.Created,
            UpdatedAt = emailAccount.Updated
        };

        return Ok(emailAccountDto);
    }
}
