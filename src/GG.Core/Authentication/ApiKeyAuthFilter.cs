using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GG.Core.Authentication;

public class ApiKeyAuthFilter(ApiKeyService apiKeysService) : IAuthorizationFilter
{
    public const string ApiKeyHeaderName = "X-Api-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string userApiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName].ToString();

        if (string.IsNullOrWhiteSpace(userApiKey))
        {
            context.Result = new BadRequestResult();
            return;
        }

        if (!apiKeysService.IsValidApiKey(userApiKey))
            context.Result = new UnauthorizedResult();
    }
}
