using GG.Core.Dto;
using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class EmailTemplateController(EmailTemplateService emailTemplateService) : AppControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create(EmailTemplateAddDto emailTemplateAddDto, CancellationToken cancellationToken)
    {
        var emailHostExists = await emailTemplateService.Exists(emailTemplateAddDto.Name, GetUserId(), cancellationToken);

        if (emailHostExists)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }

        var id = await emailTemplateService.Create(emailTemplateAddDto, GetUserId(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, emailTemplateAddDto);
    }

    [HttpGet]
    public async Task<IEnumerable<EmailTemplateGetDto>> Get(CancellationToken cancellationToken)
    {
        return await emailTemplateService.List(GetUserId(), cancellationToken);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmailTemplateGetDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var emailTemplateDto = await emailTemplateService.Get(id, GetUserId(), cancellationToken);

        if (emailTemplateDto is null)
        {
            return NotFound();
        }

        return Ok(emailTemplateDto);
    }
}
