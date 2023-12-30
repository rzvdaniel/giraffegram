using GG.Core.Dto;
using GG.Core.Entities;
using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class EmailHostController : BaseController
{
    private readonly ILogger<EmailHostController> logger;
    private readonly EmailHostService emailHostService;

    public EmailHostController(EmailHostService emailHostService, ILogger<EmailHostController> logger)
    {
        this.logger = logger;
        this.emailHostService = emailHostService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create(AddEmailHostDto emailHost, CancellationToken cancellationToken)
    {
        var emailHostExists = await emailHostService.Exists(emailHost.Name, cancellationToken);

        if (emailHostExists)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var id = await emailHostService.Add(emailHost, cancellationToken);

        return Ok(id);
    }

    [HttpGet]
    public async Task<IEnumerable<EmailHost>> Get()
    {
        return await emailHostService.List();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EmailHost> Get(Guid id)
    {
        var emailHost = emailHostService.Get(id);

        if (emailHost is null)
        {
            return NotFound();
        }

        return Ok(emailHost);
    }
}
