using GG.Core.Models;
using GG.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GG.Api.Controllers;

public class ApiKeyController(ApiKeyService apiKeyService) : AppControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create(ApiKeyAdd apiKey, CancellationToken cancellationToken)
    {
        var apiKeyExists = await apiKeyService.Exists(apiKey.Name, GetUserId(), cancellationToken);

        if (apiKeyExists)
        {
            return Conflict();
        }

        var newApiKey = await apiKeyService.Create(apiKey, GetUserId(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { newApiKey.Id }, newApiKey);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApiKeyGet>>> Get(CancellationToken cancellationToken)
    {
        var apiKeys = await apiKeyService.List(GetUserId(), cancellationToken);

        return Ok(apiKeys);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiKeyGet>> Get(Guid id, CancellationToken cancellationToken)
    {
        var apiKey = await apiKeyService.Get(id, GetUserId(), cancellationToken);

        return apiKey is not null ? Ok(apiKey) : NotFound();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Update(Guid id, ApiKeyUpdate apiKey, CancellationToken cancellationToken)
    {
        var result = await apiKeyService.Update(id, apiKey, GetUserId(), cancellationToken);

        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiKeyService.Delete(id, GetUserId(), cancellationToken);

        return result ? Ok() : NotFound();
    }
}
