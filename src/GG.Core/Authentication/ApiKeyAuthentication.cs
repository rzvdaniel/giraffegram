using Microsoft.AspNetCore.Mvc;

namespace GG.Core.Authentication;

public class ApiKeyAttribute : ServiceFilterAttribute
{
    public ApiKeyAttribute()
        : base(typeof(ApiKeyAuthFilter))
    {
    }
}