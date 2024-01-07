using GG.Core.Authentication;
using GG.Core.Dto;
using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

[ApiKey]
[ApiController]
[Route("api/[controller]")]
public class EmailController(EmailService emailService) : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "Hello!";
    }

    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Send(EmailSendDto sendEmailDto, CancellationToken cancellationToken)
    {
        await emailService.Send(sendEmailDto, Guid.NewGuid(), cancellationToken);

        return Ok();
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
    {
        var emailTemplateDto = await emailService.Get(name, Guid.NewGuid(), cancellationToken);

        if (emailTemplateDto is null)
        {
            return NotFound();
        }

        return Ok(emailTemplateDto);
    }
}
