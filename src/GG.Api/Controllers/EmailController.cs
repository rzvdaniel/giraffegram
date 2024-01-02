using GG.Auth.Controllers;
using GG.Core.Dto;
using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class EmailController(EmailService emailService) : AppControllerBase
{
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Send(SendEmailDto sendEmailDto, CancellationToken cancellationToken)
    {
        await emailService.Send(sendEmailDto, GetUserId(), cancellationToken);

        return Ok();
    }
}
