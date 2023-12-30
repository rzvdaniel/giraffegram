using GG.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class EmailController(EmailService emailService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("test")]
    public IActionResult Test()
    {
        emailService.Test();

        return Ok();
    }
}
