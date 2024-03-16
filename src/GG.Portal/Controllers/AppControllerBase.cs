using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GG.Portal.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public abstract class AppControllerBase : ControllerBase
{
    protected Guid GetUserId()
    {
        var userIdString = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Guid.TryParse(userIdString, out Guid userId);

        return userId;
    }
}