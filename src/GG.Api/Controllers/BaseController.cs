using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : Controller
{
    protected void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
    
    protected Guid GetUserId()
    {
        var userIdString = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Guid.TryParse(userIdString, out Guid userId);

        return userId;
    }
}