using GG.Core.Dto;
using GG.Core.Entities;
using GG.Core.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;

namespace GG.Api.Controllers;

public class EmailHostController : AppControllerBase
{
    private readonly ILogger<EmailHostController> logger;
    private readonly EmailHostService emailHostService;

    public EmailHostController(EmailHostService emailHostService, ILogger<EmailHostController> logger)
    {
        this.logger = logger;
        this.emailHostService = emailHostService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create(AddEmailHostDto emailHost, CancellationToken cancellationToken)
    {
        var emailHostExists = await emailHostService.Exists(emailHost.Name, GetUserId(), cancellationToken);

        if (emailHostExists)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var id = await emailHostService.Create(emailHost, GetUserId(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, emailHost);
    }

    [HttpGet]
    public async Task<IEnumerable<EmailHost>> Get(CancellationToken cancellationToken)
    {
        return await emailHostService.List(GetUserId(), cancellationToken);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmailHost>> Get(Guid id, CancellationToken cancellationToken)
    {
        var emailHost = await emailHostService.Get(id, GetUserId(), cancellationToken);

        if (emailHost is null)
        {
            return NotFound();
        }

        return Ok(emailHost);
    }
}
