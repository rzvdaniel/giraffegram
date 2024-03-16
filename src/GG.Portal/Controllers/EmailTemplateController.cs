using GG.Portal.Controllers;
using GG.Portal.Services.EmailTemplate;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class EmailTemplateController(EmailTemplateService emailTemplateService) : AppControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create(EmailTemplateAdd emailTemplateAddDto, CancellationToken cancellationToken)
    {
        var emailTemplateExists = await emailTemplateService.Exists(emailTemplateAddDto.Name, GetUserId(), cancellationToken);

        if (emailTemplateExists)
        {
            return Conflict();
        }

        var id = await emailTemplateService.Create(emailTemplateAddDto, GetUserId(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, emailTemplateAddDto);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmailTemplateGet>>> Get(CancellationToken cancellationToken)
    {
        var emailTemplates = await emailTemplateService.List(GetUserId(), cancellationToken);

        return Ok(emailTemplates);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmailTemplateGet>> Get(Guid id, CancellationToken cancellationToken)
    {
        var emailTemplate = await emailTemplateService.Get(id, GetUserId(), cancellationToken);

        return emailTemplate is not null ? Ok(emailTemplate) : NotFound();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Update(Guid id, EmailTemplateUpdate emailTemplate, CancellationToken cancellationToken)
    {
        var result = await emailTemplateService.Update(id, emailTemplate, GetUserId(), cancellationToken);

        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await emailTemplateService.Delete(id, GetUserId(), cancellationToken);

        return result ? Ok() : NotFound();
    }
}
