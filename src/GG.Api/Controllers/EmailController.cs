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
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Send(EmailSendDto sendEmailDto, CancellationToken cancellationToken)
    {
        await emailService.Send(sendEmailDto, cancellationToken);

        return Ok();
    }

    [HttpPost("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByName(string name, Dictionary<string, string> contextData, CancellationToken cancellationToken)
    {
        string userApiKey = HttpContext.Request.Headers[ApiKeyAuthFilter.ApiKeyHeaderName].ToString();

        var email = await emailService.Get(name, userApiKey, contextData, cancellationToken);

        if (email is null)
        {
            return NotFound();
        }

        return Ok(email);
    }
}
