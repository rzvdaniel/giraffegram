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
        var emailTemplateExists = await emailTemplateService.Exists(emailTemplateAddDto.Name, GetUserId(), cancellationToken);

        if (emailTemplateExists)
        {
            return Conflict();
        }

        var id = await emailTemplateService.Create(emailTemplateAddDto, GetUserId(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmailTemplateGetDto>>> Get(CancellationToken cancellationToken)
    {
        var emailTemplates = await emailTemplateService.List(GetUserId(), cancellationToken);

        return Ok(emailTemplates);
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

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Update(Guid id, EmailTemplateUpdateDto emailTemplate)
    {
        emailTemplateService.Update(id, emailTemplate, GetUserId());

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(Guid id)
    {
        emailTemplateService.Delete(id, GetUserId());

        return NoContent();
    }
}
