using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class EmailController(EmailService emailService) : AppControllerBase
{
    [HttpPost("test")]
    public IActionResult Test()
    {
        emailService.Test();

        return Ok();
    }
}
