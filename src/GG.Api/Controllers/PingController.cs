using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class PingController(ILogger<GlobalExceptionHandler> logger) : AppControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult Get()
    {
        logger.LogInformation("Pong");

        return Ok("Pong");
    }
}
