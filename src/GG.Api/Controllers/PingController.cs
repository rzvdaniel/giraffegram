using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class PingController : AppControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public ActionResult Get()
    {
        return Ok("Pong");
    }
}
